using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UrlsProject.AppDataBase.Entities;
using UrlsProject.AppDataBase.Repository;
using UrlsProject.Common.Models;
using UrlsProject.Helpers;

namespace UrlsProject.Services
{
    public class HomeControllerService: IHomeControllerService
    {
        private readonly SiteCollector _siteCollector;
        private readonly IAppRepository _repository;
        
        public HomeControllerService()
        {
            _siteCollector = new SiteCollector();
            _repository = new AppRepository();
        }

        public async Task<PageResultInfoViewModel> Results(string url)
        {
            try
            {
                url = Helper.RemoveAfter(url);

                if (string.IsNullOrEmpty(url))
                {
                    return PageResultInfoViewModel.ErrorWithResult(", Url is empty");
                }

                if (!await Helper.ExistUrl(url))
                {
                    return PageResultInfoViewModel.ErrorWithResult(url);
                }

                IEnumerable<SiteInfoModel> siteInfos = await _siteCollector.Collect(url);
                IEnumerable<PageInfoModel> pagesInfo = await AddUpdateResults(siteInfos);

                return GetPageInfoView(pagesInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }                  
        }

        private async Task<IEnumerable<PageInfoModel>> AddUpdateResults
            (IEnumerable<SiteInfoModel> siteInfos)
        {
            if (!siteInfos.Any())
            {
                return null;
            }

            try
            {
                if (await ExistInDb(siteInfos.First().Url))
                {
                    return await UpdateHistoryGetPagesInfo(siteInfos);
                }
                else
                {
                    return await AddHostGetPagesInfo(siteInfos);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }
            
        private async Task<IEnumerable<PageInfoModel>> AddHostGetPagesInfo
            (IEnumerable<SiteInfoModel> siteInfos)
        {
            var resultInfoPage = new List<PageInfoModel>(siteInfos.Count());

            var host = new Host
            {
                NameOfHost = Helper.GetHost(siteInfos.First().Url)
            };

            foreach (SiteInfoModel page in siteInfos)
            {
                var pageInHost = new HostPage
                {
                    Url = page.Url,
                    MinResponse = page.ResponseTime,
                    MaxResponse = page.ResponseTime
                };

                resultInfoPage.Add(new PageInfoModel
                {
                    Url = page.Url,
                    MinResponse = page.ResponseTime,
                    MaxResponse = page.ResponseTime,
                    ResponseTime = page.ResponseTime
                });

                pageInHost.HostHistories.Add(new HostHistory
                {
                    Date = page.Date,
                    ResponseTime = page.ResponseTime
                });

                host.HostPages.Add(pageInHost);
            }

            _repository.Add(host);
            await _repository.SaveChanges();

            return resultInfoPage;
        }

        private async Task<IEnumerable<PageInfoModel>> UpdateHistoryGetPagesInfo
            (IEnumerable<SiteInfoModel> siteInfos)
        {
            var resultInfoPage = new List<PageInfoModel>(siteInfos.Count());

            string hostUrl = Helper.GetHost(siteInfos.First().Url);
            Host host = await _repository.GetHostIncludePages(hostUrl);

            foreach (SiteInfoModel page in siteInfos)
            {
                HostPage pageUpdate = host.HostPages.FirstOrDefault(x => x.Url == page.Url);
                if (pageUpdate != null)
                {
                    if (pageUpdate.MinResponse > page.ResponseTime)
                    {
                        pageUpdate.MinResponse = page.ResponseTime;
                    }
                    if (pageUpdate.MaxResponse < page.ResponseTime)
                    {
                        pageUpdate.MaxResponse = page.ResponseTime;
                    }

                    pageUpdate.HostHistories.Add(new HostHistory
                    {
                        Date = page.Date,
                        ResponseTime = page.ResponseTime
                    });

                    resultInfoPage.Add(new PageInfoModel
                    {
                        Url = page.Url,
                        MinResponse = pageUpdate.MinResponse,
                        MaxResponse = pageUpdate.MaxResponse,
                        ResponseTime = page.ResponseTime
                    });

                    _repository.Update(pageUpdate);
                }
                else
                {
                    var pageNew = new HostPage
                    {
                        Url = page.Url,
                        MinResponse = page.ResponseTime,
                        MaxResponse = page.ResponseTime
                    };

                    pageNew.HostHistories.Add(new HostHistory
                    {
                        Date = page.Date,
                        ResponseTime = page.ResponseTime
                    });

                    resultInfoPage.Add(new PageInfoModel
                    {
                        Url = page.Url,
                        MinResponse = page.ResponseTime,
                        MaxResponse = page.ResponseTime,
                        ResponseTime = page.ResponseTime
                    });

                    host.HostPages.Add(pageNew);

                    _repository.Update(host);
                }
            }

            await _repository.SaveChanges();
            return resultInfoPage;
        }

        public async Task<HistoryViewModel> GetHostHistory(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
            {
                return HistoryViewModel.HistoryError(hostUrl, "Please enter url");
            }
            
            var gruped = (await _repository.GetHostHistory(hostUrl)).GroupBy(x => x.Page.Url).ToList();

            if (!gruped.Any())
            {
                return HistoryViewModel.HistoryError(hostUrl, "History is empty");
            }

            var history = new HistoryViewModel
            {
                HostUrl = hostUrl,
                Success = true,
                HostPages = new List<PageModel>()
            };

            foreach (IGrouping<string, HostHistory> page in gruped)
            {
                var newPage = new PageModel
                {
                    Url = page.Key,
                    HostHistory = page.OrderByDescending(x => x.Date).Select(x => new HistoryModel
                    {
                        Date = x.Date.ToString("MM/dd/yyyy HH:mm:ss"),
                        ResponseTime = x.ResponseTime
                    }).ToList()
                };

                history.HostPages.Add(newPage);
            }

            return history;
        }

        private async Task<bool> ExistInDb(string url)
        {
            try
            {
                string hostUrl = Helper.GetHost(url);
                Host findedHostUrl = await _repository.GetHost(hostUrl);
                
                if(findedHostUrl == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PageResultInfoViewModel GetPageInfoView(IEnumerable<PageInfoModel> pagesInfo)
        {
            var resultInfoView = new PageResultInfoViewModel();

            if (pagesInfo != null)
            {
                resultInfoView.Pages = pagesInfo.OrderByDescending(x => x.ResponseTime).ToList();
                resultInfoView.AverageResponse = pagesInfo.Average(x => x.ResponseTime);
            }

            if(resultInfoView.Pages == null)
            {
                resultInfoView.Success = false;
            }
            else
            {
                resultInfoView.Success = true;
            }
            
            if (resultInfoView.Success)
            {
                resultInfoView.HostUrl = Helper.GetHost(pagesInfo.First().Url);
            }
            else
            {
                resultInfoView.HostUrl = string.Empty;
            }

            return resultInfoView;
        }
    }
}