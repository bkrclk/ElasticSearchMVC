using ElasticSearchMVC.Helpers;
using ElasticSearchMVC.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchMVC.Utils
{
    public static class ElasticModelAccessor
    {
        public static IndexResponse AddModelToIndex<T>(T model, string indexName = null) where T : class
        {
            return ElasticHelper<T>.Instance.AddIndex(model, indexName?.ToLower() ?? typeof(T).Name.ToLower());
        }

        public static BulkResponse AddModelListToIndex<T>(List<T> models, string indexName = null) where T : class
        {
            return ElasticHelper<T>.Instance.BulkIndexList(models, indexName?.ToLower() ?? typeof(ProductModel).Name.ToLower());
        }

        public static DeleteResponse DeleteModelToIndex<T>(string id) where T: class
        {
            return ElasticHelper<T>.Instance.DeleteIndex(typeof(T).Name.ToLower(), id);
        }
    }
}
