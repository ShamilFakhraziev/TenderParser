using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TenderParser.Models;
using TenderParser.ViewModels;

namespace TenderParser.Services
{
    public static class Parser
    {
        public static async Task<TenderVM> GetTenderAsync(int id)
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response;

                    //создаю тело для последующего запроса, сериализую данные в json и конвертирую их в HttpContent, который нужен для отправки запроса
                    var requestBody = JsonConvert.SerializeObject(new { page = 1, itemsPerPage = 1, id = id }, Formatting.Indented);
                    var convertedRequestBody = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    //отправляю запрос, в качестве контента передаю созданный ранее body и после выполнения запроса проверяю его на успешность выполнения
                    response = await client.PostAsync("https://api.market.mosreg.ru/api/Trade/GetTradesForParticipantOrAnonymous", convertedRequestBody);

                    if (!response.IsSuccessStatusCode) return null;

                    //в качестве ответа после запроса прилетает json ответ и я представляю его в виде строки и после десериализую
                    var tenderBaseInfo = JsonConvert.DeserializeObject<TenderBaseInfo>(await response.Content.ReadAsStringAsync());

                    if (tenderBaseInfo.totalRecords == "0")
                    {
                        return null;
                    }
                    //создаю экземпляр тендера, и заполняю его базовыми данными, в конце выполнения функции я верну этот заполненный экземпляр
                    Tender tender = new Tender();
                    tender.Id = tenderBaseInfo.invData[0].Id;
                    tender.TradeName = tenderBaseInfo.invData[0].TradeName;
                    tender.TradeStateName = tenderBaseInfo.invData[0].TradeStateName;
                    tender.CustomerFullName = tenderBaseInfo.invData[0].CustomerFullName;
                    tender.InitialPrice = tenderBaseInfo.invData[0].InitialPrice;
                    tender.PublicationDate = tenderBaseInfo.invData[0].PublicationDate.AddHours(3);
                    tender.FillingApplicationEndDate = tenderBaseInfo.invData[0].FillingApplicationEndDate.AddHours(3);


                    //отпраляю запрос на получение страницы извещения тендера
                    response = await client.GetAsync($"https://market.mosreg.ru/Trade/ViewTrade/{id}");

                    if (!response.IsSuccessStatusCode) return null;

                    //в качестве ответа на запрос отправленный ранее прилетит html разметка, чтобы распарсить ее и получить необходимые данные
                    //я использовал библиотеку HtmlAgilityPack
                    var viewTradeHtml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    //декодирую символы, чтобы не было такого: &quot;
                    var result = HttpUtility.HtmlDecode(viewTradeHtml);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(result);
                    var nodes = doc.DocumentNode.SelectNodes("//section[@class='informationAboutCustomer']/div[@class='wrapper']/div");
                    tender.DeliveryPlace = nodes[2].SelectNodes("div")[5].SelectSingleNode("p").InnerText.Trim();

                    //получаю все обьекты закупки тендера
                    var purchaseObjects = nodes[3].SelectNodes("div/div");
                    List<TenderLot> lotPos = new List<TenderLot>();
                    foreach (var obj in purchaseObjects)
                    {
                        TenderLot lot = new TenderLot();
                        lot.Name = obj.SelectNodes("div[@class='outputResults__oneResult-leftPart leftPart']/p")[0].InnerText;
                        lot.UnitOfMeasurement = obj.SelectNodes("div[@class='outputResults__oneResult-centerPart centerPart']/div/p")[0].InnerText;
                        lot.Count = obj.SelectNodes("div[@class='outputResults__oneResult-centerPart centerPart']/div/p")[1].InnerText;
                        lot.UnitPrice = obj.SelectSingleNode("div[@class='outputResults__oneResult-rightPart rightPart']/div/p").InnerText;
                        lotPos.Add(lot);
                    }
                    tender.LotPositionsList = lotPos;

                    //получаю документы тендера
                    response = await client.GetAsync($"https://api.market.mosreg.ru/api/Trade/{id}/GetTradeDocuments");

                    if (!response.IsSuccessStatusCode) return null;
                    tender.DocumentsList = JsonConvert.DeserializeObject<List<TenderDocument>>(response.Content.ReadAsStringAsync().Result);

                    TenderVM tenderVM = new TenderVM { tender = tender, exception = null };
                    return tenderVM;
                }
            }
            catch (Exception ex)
            {
                return new TenderVM { tender = null, exception = ex };
            }


        }
    }
}
