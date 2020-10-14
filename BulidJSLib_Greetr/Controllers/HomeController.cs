using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulidJSLib_Greetr.Models;
using System.Drawing.Imaging;
using System.IO;
using Novacode;
using System.Threading;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace BulidJSLib_Greetr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        public HomeController(ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ExportDocx(List<string> imgDatas)
        {
            try
            {
                var result = ExportDocxFile(imgDatas);
                string FileGuid = new Guid().ToString();              
                var _fullPath = Path.Combine($"{_hostingEnvironment.WebRootPath}", "測試.docx");
                FileStream file = new FileStream(_fullPath, FileMode.Create, FileAccess.Write);
                result.Position = 0;
                result.WriteTo(file);
               
                file.Close();

                var resultObj = new
                {
                    FileName = $@"測試",
                    Successful = true
                };
                return Json(resultObj);
               
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

           // return View();
        }
        public IActionResult HighChartDocxDownload(string fileName)
        {
            using (var document = DocX.Load($@"{_hostingEnvironment.WebRootPath}/{fileName}.docx"))
            {
                var memory = new MemoryStream();

                document.SaveAs(memory);
                memory.Position = 0;
                return File(memory.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"測試2.docx");
            }
     
        }

        public MemoryStream ExportDocxFile(List<string> imgDatas)
        {          
            using (var document = DocX.Load($@"{_hostingEnvironment.WebRootPath}/ChartTest.docx"))
            {
                var imageTableIdx = 1;//定義第一張圖的表格位置
                var ImageTableIndex = new List<int>() { 1, 3, 5, 7, 8, 9 };
                var failImage = new List<int>();
                using (var ms = new MemoryStream())
                {
                    var mmsg = "";
                    var imageTable = document.Tables[0];//
                    foreach (var imgData in imgDatas)
                    {
                        using (var Img = GenerateHighChartImage(imgData, ref mmsg))
                        {
                            
                            if (Img != null)
                            {
                               
                                Img.Save(ms, ImageFormat.Png);//Save your picture in a memory stream.
                                ms.Seek(0, SeekOrigin.Begin);
                                Novacode.Image img = document.AddImage(ms);

                                //Paragraph p = document.InsertParagraph("Hello", false);

                                Picture pic1 = img.CreatePicture();     
                                var row = imageTable.Rows[0];
                                row.MergeCells(0, 1);
                                var MaxWidth = row.Cells[0].Width;
                                var ratio = MaxWidth / Img.Width;
                                var width = Math.Round((double)Img.Width * 0.35);
                                var height = Math.Round((double)Img.Height * 0.35);
                                row.Cells[0].Paragraphs[0].Alignment = Alignment.center;
                                row.Cells[0].Paragraphs[0].InsertPicture(pic1, 0);

                                row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                                // imageTable.Alignment = Alignment.center;
                                //imageTableIdx += 2;
                            }

                        }
                    }
                    
                }
                var memory = new MemoryStream();

                document.SaveAs(memory);

                return memory;
                //return FileTool.ConvertDocToMemoryStream(document);
            }
        }
        public System.Drawing.Image GenerateHighChartImage(string option, ref string mmsg)
        {
            option = option.Replace("\r\n", "");
            option = option.Replace(" ", "");
            var request = (HttpWebRequest)WebRequest.Create("https://export.highcharts.com/");
            System.Drawing.Image ResultImg;
            var bytes = Encoding.UTF8.GetBytes(option);
            //因應關閉TLS1.0與TLS1.2
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.ContentLength = bytes.Length;

            //var mmsg = "";
            var ErrorMessage = "";
            try
            {
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                //var ResultImg = new System.Drawing.Image();
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    mmsg = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    if (!string.IsNullOrEmpty(mmsg))
                    {                        
                        var requestPic = WebRequest.Create("https://export.highcharts.com/" + mmsg);
                        using (WebResponse responsePic = requestPic.GetResponse())
                        {
                            using (var webImage = System
                                .Drawing
                                .Image
                                .FromStream(responsePic.GetResponseStream()))
                            {
                                ResultImg = (System.Drawing.Image)webImage.Clone();
                                return ResultImg;
                            }
                        }
                    }
                }

            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                //Exception exception = exception1;
                ErrorMessage = exception1.Message;
                // ProjectData.ClearProjectError();
            }

            ResultImg = null;
            mmsg = ErrorMessage;
            return ResultImg;
        }
    }
}
//if (mmsg.Contains("429"))//429:Too Many Request，表示短時間內傳送太多Request，此時等待2秒後重新傳一次
//                            {
//                                Thread.Sleep(2000); //Delay 2秒
//                                using (var ImgAgain = GenerateHighChartImage(imgData, ref mmsg))
//                                {
//                                    if (ImgAgain != null)
//                                    {
//                                        ImgAgain.Save(ms, ImageFormat.Png);//Save your picture in a memory stream.
//                                        ms.Seek(0, SeekOrigin.Begin);
//                                        Novacode.Image img = document.AddImage(ms);

////Paragraph p = document.InsertParagraph("Hello", false);

//Picture pic1 = img.CreatePicture();     // Create picture.
//var width = Math.Round((double)ImgAgain.Width * 0.35);
//var height = Math.Round((double)ImgAgain.Height * 0.35);                          //pic1.SetPictureShape(BasicShapes.); // Set picture shape (if needed)
//var row = imageTable.Rows[0];
//row.MergeCells(0, 1);//圖1、喪葬慰問金各縣市發放件數及金額統計圖
//                                                             //pic1.Width = (int)p1.GetColumnWidth(0);
//                                                             //p.InsertPicture(pic1, 0); // Insert picture into paragraph.
//                                                             //row.Cells[0].Paragraphs[0].Append(ChartNameDic[i.ToString()]);
//                                        row.Cells[0].Paragraphs[0].Alignment = Alignment.center;
//                                        row.Cells[0].Paragraphs[0].InsertPicture(pic1, 0);

//row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
//                                        //imageTableIdx += 2;
//                                    }

//                                };
//                            }