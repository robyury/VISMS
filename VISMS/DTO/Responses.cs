using System.Text.Json.Serialization;

namespace VISMS.DTO
{
    public class Response
    {
        public string service_code { get; set; }

        public int Result { get; set; }

        public string msg { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? bonus_balance { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? real_balance { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? total_product_count { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? product_array_length { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<VISMS_ProductItem>? product_list { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OrderID { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OrderNo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PaymentRuleID { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ProductArrayLength { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<VISMS_ProductInfo>? ProductInfo { get; set; }


    }

    public class VISMS_ProductItem
    {
        public int product_no { get; set; }
        public int relation_product_no { get; set; }
        public int product_expire { get; set; }
        public int product_pieces { get; set; }
        public int payment_type { get; set; }
        public int sale_price { get; set; }
        public int category_no { get; set; }
        public int bonus_product_count { get; set; }
        public string product_id { get; set; }
        public string product_guid { get; set; }
        public string product_type { get; set; }
    }

    public class VISMS_ProductInfo
    {
        public int product_no { get; set; }
        public int order_quantity { get; set; }
    }

}
