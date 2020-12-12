using ElasticSearchMVC.Context;
using ElasticSearchMVC.Helpers;
using ElasticSearchMVC.Models;
using ElasticSearchMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ElasticSearchMVC.Controllers
{
    public class ProductController : Controller
    {
        private ProductContext Context;
        private readonly ILogger<ProductController> Logger;
        public static List<ProductModel> Products = new List<ProductModel>();
        public static BulkResponse bulkResponse;

        public ProductController(ILogger<ProductController> logger, ProductContext context)
        {
            Logger = logger;
            Context = context;
            //ProductsBind();
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (Products.Count == 0 || Products == null)
                Products = Context.Set<ProductModel>().ToList();
            return View(Products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            bulkResponse = ElasticModelAccessor.AddModelListToIndex(Products);
            return View();
        }

        public IActionResult Create(ProductModel elasticModel)
        {
            //Eğer yeni bir index değilde var olan index içerisne eklenmek istenirse "productList" indexi belirtilir.
            IndexResponse indexResponse = ElasticModelAccessor.AddModelToIndex(elasticModel);

            if (indexResponse.IsValid || indexResponse.Result == Result.Created)
            {
                ISearchResponse<ProductModel> searchResponses = ElasticHelper<ProductModel>.Instance.Search(p => p
                    .DateRange(r => r
                        .Field(f => f.ProductionDate)
                        .GreaterThanOrEquals(new DateTime(2020, 01, 01))
                        .LessThanOrEquals(new DateTime(2021, 01, 01))
                    ), typeof(ProductModel).Name.ToLower()
                );

                var responseViewModel = new List<ProductModel>();
                foreach (var searchResponse in searchResponses.Documents)
                {
                    responseViewModel.Add(new ProductModel()
                    {
                        Id = searchResponse.Id,
                        ProductName = searchResponse.ProductName,
                        Description = searchResponse.Description,
                        Price = searchResponse.Price
                    });
                }
            }

            Context.Add(elasticModel);
            Context.SaveChanges();
            return View();
        }

        [HttpPost]
        public IActionResult Search(string searchText)
        {
            if (bulkResponse.IsValid || bulkResponse.Errors)
            {
                ISearchResponse<ProductModel> searchResponses = ElasticHelper<ProductModel>.Instance.Search(p => p
                    .MatchPhrasePrefix(y => y
                        .Field(z => z.ProductName).Query(searchText)
                        .Field(x => x.Description).Query(searchText)
                    ),
                    typeof(ProductModel).Name.ToLower()
                );

                var searchViewModel = new List<ProductModel>();
                foreach (var searchResponse in searchResponses.Documents)
                {
                    searchViewModel.Add(new ProductModel()
                    {
                        Id = searchResponse.Id,
                        ProductName = searchResponse.ProductName,
                        Description = searchResponse.Description,
                        Price = searchResponse.Price,
                        Image = searchResponse.Image
                    });
                }
                Products = searchViewModel;
            }
            return RedirectToAction("Index", "Product", Products);
        }

        [HttpPost]
        public IActionResult DeleteOnIndex(string id)
        {
            DeleteResponse deleteResponse = ElasticModelAccessor.DeleteModelToIndex<ProductModel>(id);

            if (deleteResponse.IsValid || deleteResponse.Result == Result.Deleted)
            {
                ISearchResponse<ProductModel> searchResponses = ElasticHelper<ProductModel>.Instance.Search(p => p
                    .DateRange(r => r
                        .Field(f => f.ProductionDate)
                        .GreaterThanOrEquals(new DateTime(2020, 01, 01))
                        .LessThanOrEquals(new DateTime(2021, 01, 01))
                    )
                );
                var deleteViewModel = new List<ProductModel>();
                foreach (var searchResponse in searchResponses.Documents)
                {
                    deleteViewModel.Add(new ProductModel()
                    {
                        Id = searchResponse.Id,
                        ProductName = searchResponse.ProductName,
                        Description = searchResponse.Description
                    });
                }

            }
            var delProduct = Context.ProductModel.First(c => c.Id == Convert.ToInt32(id));
            Context.Remove(delProduct);
            Context.SaveChanges();

            return RedirectToAction("Index", "Product", Products);
        }

        public IActionResult LogIndex()
        {
            try
            {
                int a = 0;
                int b = 10 / a;
            }
            catch (Exception ex)
            {
                ElasticLogger.Instance.Error(ex, ex.Message);
            }
            finally
            {
                ElasticLogger.Instance.Info("ElasticLoggerTestMethod işlemlerini gerçekleştirdi.");
            }

            return RedirectToAction("Index", "Product");
        }

        //product.json listesini DB ye kaydetmek için bu metod çalıştırılabilir.
        //public void ProductsBind()
        //{
        //    if (Products == null || Products.Count == 0)
        //    {
        //        var path = System.IO.Path.GetFullPath(".\\wwwroot\\product.json");
        //        var webClient = new WebClient();
        //        var json = webClient.DownloadString(path);
        //        Products = JsonConvert.DeserializeObject<List<ProductModel>>(json);
        //    }
        //    foreach (var item in Products)
        //    {
        //        Context.Add(item);
        //        Context.SaveChanges();
        //    }
        //}
    }
}
