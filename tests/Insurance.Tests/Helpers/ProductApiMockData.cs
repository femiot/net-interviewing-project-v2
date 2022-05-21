using Insurance.Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Insurance.Tests.Helpers
{
    public static class ProductApiMockData
    {
        public static List<ProductIntegrationDto> GetProducts()
        {
            var productsJson = "[\r\n  {\r\n    \"id\": 572770,\r\n    \"name\": \"Samsung WW80J6400CW EcoBubble\",\r\n    \"salesPrice\": 475,\r\n    \"productTypeId\": 124\r\n  },\r\n  {\r\n    \"id\": 715990,\r\n    \"name\": \"Canon Powershot SX620 HS Black\",\r\n    \"salesPrice\": 195,\r\n    \"productTypeId\": 33\r\n  },\r\n  {\r\n    \"id\": 725435,\r\n    \"name\": \"Cowon Plenue D Gold\",\r\n    \"salesPrice\": 299.99,\r\n    \"productTypeId\": 12\r\n  },\r\n  {\r\n    \"id\": 735246,\r\n    \"name\": \"AEG L8FB86ES\",\r\n    \"salesPrice\": 699,\r\n    \"productTypeId\": 124\r\n  },\r\n  {\r\n    \"id\": 735296,\r\n    \"name\": \"Canon EOS 5D Mark IV Body\",\r\n    \"salesPrice\": 2699,\r\n    \"productTypeId\": 35\r\n  },\r\n  {\r\n    \"id\": 767490,\r\n    \"name\": \"Canon EOS 77D + 18-55mm IS STM\",\r\n    \"salesPrice\": 749,\r\n    \"productTypeId\": 35\r\n  },\r\n  {\r\n    \"id\": 780829,\r\n    \"name\": \"Panasonic Lumix DC-TZ90 Silver\",\r\n    \"salesPrice\": 319,\r\n    \"productTypeId\": 33\r\n  },\r\n  {\r\n    \"id\": 805073,\r\n    \"name\": \"Haier HW80-B14636\",\r\n    \"salesPrice\": 449,\r\n    \"productTypeId\": 124\r\n  },\r\n  {\r\n    \"id\": 819148,\r\n    \"name\": \"Nikon D3500 + AF-P DX 18-55mm f/3.5-5.6G VR\",\r\n    \"salesPrice\": 469,\r\n    \"productTypeId\": 35\r\n  },\r\n  {\r\n    \"id\": 827074,\r\n    \"name\": \"Samsung Galaxy S10 Plus 128 GB Black\",\r\n    \"salesPrice\": 699,\r\n    \"productTypeId\": 32\r\n  },\r\n  {\r\n    \"id\": 828519,\r\n    \"name\": \"Huawei P30 Lite 128 GB Black\",\r\n    \"salesPrice\": 219,\r\n    \"productTypeId\": 32\r\n  },\r\n  {\r\n    \"id\": 832845,\r\n    \"name\": \"Apple iPod Touch (2019) 32 GB Space Gray\",\r\n    \"salesPrice\": 229,\r\n    \"productTypeId\": 12\r\n  },\r\n  {\r\n    \"id\": 836194,\r\n    \"name\": \"Sony CyberShot DSC-RX100 VII\",\r\n    \"salesPrice\": 1129,\r\n    \"productTypeId\": 33\r\n  },\r\n  {\r\n    \"id\": 836676,\r\n    \"name\": \"Sandisk Clip Sport Plus Blue\",\r\n    \"salesPrice\": 74.99,\r\n    \"productTypeId\": 12\r\n  },\r\n  {\r\n    \"id\": 837856,\r\n    \"name\": \"Lenovo Chromebook C330-11 81HY000MMH\",\r\n    \"salesPrice\": 299,\r\n    \"productTypeId\": 21\r\n  },\r\n  {\r\n    \"id\": 838978,\r\n    \"name\": \"Asus ZenBook UX334FAC-A3066T\",\r\n    \"salesPrice\": 1149,\r\n    \"productTypeId\": 841\r\n  },\r\n  {\r\n    \"id\": 858421,\r\n    \"name\": \"Lenovo IdeaPad L340-15IRH Gaming 81LK01FUMH\",\r\n    \"salesPrice\": 779,\r\n    \"productTypeId\": 21\r\n  },\r\n  {\r\n    \"id\": 859366,\r\n    \"name\": \"OnePlus 8 Pro 128GB Black 5G\",\r\n    \"salesPrice\": 886,\r\n    \"productTypeId\": 32\r\n  },\r\n  {\r\n    \"id\": 861866,\r\n    \"name\": \"Apple MacBook Pro 13\\\" (2020) MXK52N A Space Gray\",\r\n    \"salesPrice\": 1749,\r\n    \"productTypeId\": 21\r\n  }\r\n]";
            var products = JsonSerializer.Deserialize<List<ProductIntegrationDto>>(productsJson);
            products.Add(new ProductIntegrationDto { Id = 5555, ProductTypeId = 5555, SalesPrice = 1000 });
            return products;
        }

        public static ProductIntegrationDto GetProductById(int productId)
        {
            return GetProducts().FirstOrDefault(x => x.Id == productId);
        }

        public static List<ProductTypeIntegrationDto> GetProductTypes()
        {
            var productTypesJson = "[\r\n  {\r\n    \"id\": 21,\r\n    \"name\": \"Laptops\",\r\n    \"canBeInsured\": true\r\n  },\r\n  {\r\n    \"id\": 32,\r\n    \"name\": \"Smartphones\",\r\n    \"canBeInsured\": true\r\n  },\r\n  {\r\n    \"id\": 33,\r\n    \"name\": \"Digital cameras\",\r\n    \"canBeInsured\": true\r\n  },\r\n  {\r\n    \"id\": 35,\r\n    \"name\": \"SLR cameras\",\r\n    \"canBeInsured\": false\r\n  },\r\n  {\r\n    \"id\": 12,\r\n    \"name\": \"MP3 players\",\r\n    \"canBeInsured\": false\r\n  },\r\n  {\r\n    \"id\": 124,\r\n    \"name\": \"Washing machines\",\r\n    \"canBeInsured\": true\r\n  },\r\n  {\r\n    \"id\": 841,\r\n    \"name\": \"Laptops\",\r\n    \"canBeInsured\": false\r\n  }\r\n]";
            return JsonSerializer.Deserialize<List<ProductTypeIntegrationDto>>(productTypesJson);
        }

        public static ProductTypeIntegrationDto GetProductTypeById(int productTypeId)
        {
            return GetProductTypes().FirstOrDefault(x => x.Id == productTypeId);
        }
    }
}
