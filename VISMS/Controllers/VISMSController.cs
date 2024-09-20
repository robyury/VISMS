using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VISMS.Data;
using VISMS.DTO;
using VISMS.Models;
using System.Text.Json;

namespace VISMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VISMSController : ControllerBase
    {
        private readonly ILogger<VISMSController> _logger;
        private readonly VISMSDBContext _context;

        public VISMSController(ILogger<VISMSController> logger, VISMSDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        //Validação de dados básicos da request
        private async Task<IActionResult> ValidateRequest(string service_code, string ipAddress)
        {
            if (string.IsNullOrEmpty(service_code))
                return BadRequest(new Response { service_code = service_code, Result = -97, msg = "ERD_VISMS_BILL_NOT_REGISTERED" });

            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest(new Response { service_code = service_code, Result = -99, msg = "ERD_VISMS_BILL_NOT_ALLOWED" });

            var isServiceValid = await _context.VISMS_ServiceList
                .AnyAsync(s => s.ServiceCode == service_code);
            if (!isServiceValid)
                return BadRequest(new Response { service_code = service_code, Result = -97, msg = "ERD_VISMS_BILL_NOT_REGISTERED" });

            var isAllowedServer = await _context.VISMS_AllowedServerList
                .AnyAsync(s => s.ServiceCode == service_code && s.IPAddr == ipAddress);
            if (!isAllowedServer)
                return BadRequest(new Response { service_code = service_code, Result = -99, msg = "ERD_VISMS_BILL_NOT_ALLOWED" });

            return null; 
        }

        // Função pra validar se o usuário existe
        private async Task <bool> IsValidUser(string serviceCode, string strNexonID)        
        {
            var user = await _context.VISMS_UserList
                .FirstOrDefaultAsync(s => s.ServiceCode == serviceCode && s.strNexonID == strNexonID);
            return user != null;
        }

        // Função que obtém o cash do usuário
        private async Task<(int? RealBalance, int? BonusBalance)> GetUserCash(string serviceCode, string user_id)       
        {
            var balances = await _context.VISMS_UserList
                .Where(s => s.ServiceCode == serviceCode && s.strNexonID == user_id)
                .Select(s => new
                {
                    RealBalance = s.RealBalance,
                    BonusBalance = s.BonusBalance
                })
                .FirstOrDefaultAsync();
            return (balances?.RealBalance, balances?.BonusBalance);
        }

        //Checagem de funcionamento do serviço
        [HttpPost("heartbeat")]
        public async Task<IActionResult> Heartbeat([FromForm] string service_code)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var validationResult = await ValidateRequest(service_code, ipAddress);
            if (validationResult != null)
                return validationResult;

            var response = new Response
            {
                service_code = service_code,
                Result = 1,
                msg = "",
            };

            return Ok(response);
        }

        //Verificar balanço do usuário
        [HttpPost("checkbalance")]
        public async Task<IActionResult> CheckBalance([FromForm] string service_code, [FromForm] string user_id)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var validationResult = await ValidateRequest(service_code, ipAddress);
            if (validationResult != null)
                return validationResult;

            var response = new Response
            {
                service_code = service_code,
                Result = 1,
                msg = "",
                bonus_balance = null,
                real_balance = null,

            };

            if(string.IsNullOrEmpty(user_id)) {
                response.Result = -96;
                response.msg = "ERD_VISMS_BILL_NOTUSER_ID";
                return BadRequest(response);
            }

            //Verifica se o usuário existe
            bool isValidUser = await IsValidUser(service_code, user_id);

            if(!isValidUser)
            {
                response.Result = -96;
                response.msg = "ERD_VISMS_BILL_NOTUSER_ID";
                return BadRequest(response);
            }

            var (real_balance, bonus_balance) = await GetUserCash(service_code, user_id);
            response.real_balance = real_balance;
            response.bonus_balance = bonus_balance;

            return Ok(response);
        }
        [HttpPost ("product_inquiry")]
        public async Task<IActionResult> ProductInquiry([FromForm] string service_code,[FromForm] int page_index, [FromForm] int row_per_page)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var validationResult = await ValidateRequest(service_code, ipAddress);
            if (validationResult != null)
                return validationResult;

            var query = _context.VISMS_ProductList
                                .Where(p => p.ServiceCode == service_code);

            var totalProductCount = await query.CountAsync();

            var remainingProductCount = totalProductCount - ((page_index - 1) * row_per_page);

            var productlist = await query
                                    .OrderBy(p => p.ProductNo)
                                    .Skip((page_index - 1) * row_per_page)
                                    .Take(row_per_page)
                                    .ToListAsync();

            var product_list = productlist.Select(p => new VISMS_ProductItem
            {
                product_no = p.ProductNo,
                relation_product_no = p.RelationProductNo,
                product_expire = p.ProductExpire,
                product_pieces = p.ProductPieces,
                payment_type = p.PaymentType,
                sale_price = p.SalePrice,
                category_no = p.CategoryNo,
                bonus_product_count = p.BonusProductCount,
                product_id = p.ProductID,
                product_guid = p.ProductGUID,
                product_type = p.ProductType,
            }).ToList();

            var response = new
            {
                service_code = service_code,
                Result = 1,
                total_product_count = remainingProductCount,
                product_array_length = product_list.Count,
                product_list = product_list
            };

            return Ok(response);
        }
        [HttpPost ("purchase_item_rule_id")]
        public async Task<IActionResult> PurchaseItemRuleID([FromForm] string service_code,[FromForm] string ip,[FromForm] int reason,[FromForm] string user_id, [FromForm] int user_oid,[FromForm] int user_age,[FromForm] string order_id,[FromForm] int payment_type,[FromForm] int payment_rule_id,[FromForm] int total_amount,[FromForm] int product_array_length,[FromForm] string product_info)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var validationResult = await ValidateRequest(service_code, ipAddress);
            if (validationResult != null)
                return validationResult;

            var response = new Response
            {
                service_code = service_code,
                Result = 1,
                msg = "",
                bonus_balance = null,
                real_balance = null,

            };

            if (string.IsNullOrEmpty(user_id))
            {
                response.Result = -96;
                response.msg = "ERD_VISMS_BILL_NOTUSER_ID";
                return BadRequest(response);
            }

            var user = await _context.VISMS_UserList
                .FirstOrDefaultAsync(u => u.ServiceCode == service_code && u.strNexonID == user_id);
            if (user == null)
            {
                response.Result = -96;
                response.msg = "ERD_VISMS_BILL_NOTUSER_ID";
                return BadRequest(response);
            }

            if (user.RealBalance < total_amount)
            {
                response.Result = 12040;
                response.msg = "ERD_VISMS_BILL_INSUFFICIENT_BALANCE";
                return BadRequest(response);
            }
            var productInfo = JsonSerializer.Deserialize<List<VISMS_ProductInfo>>(product_info);
            if (productInfo != null && productInfo.Count > 0)
            {
                var productArray = productInfo.Select(p => new
                {
                    product_no = p.product_no,
                    order_quantity = p.order_quantity
                }).ToList();

                var purchaseLog = new VISMS_PurchaseLog
                {
                    ServiceCode = service_code,
                    OrderID = order_id,
                    ProductNo = productArray.First().product_no,
                    PaymentType = payment_type,
                    PaymentRuleID = payment_rule_id,
                    TotalPrice = total_amount,
                    OrderAmmount = productArray.First().order_quantity,
                    oid = user_oid,
                    strNexonID = user_id,
                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IsGift = false,
                    Receiver_oid = null,
                    Receiver_strNexonID = null
                };

                var logId = await _context.VISMS_PurchaseLog.AddAsync(purchaseLog);
                await _context.SaveChangesAsync();

                // Atualize o saldo apenas se nenhuma outra falha ocorreu até agora.
                user.RealBalance -= total_amount;
                _context.VISMS_UserList.Update(user);
                await _context.SaveChangesAsync();

                response.Result = 1;
                response.msg = "ERD_VISMS_BILL_PAID_SUCCESS";
                response.OrderID = order_id;
                response.OrderNo = logId.Entity.SRL;
                response.PaymentRuleID = payment_rule_id;
                response.ProductArrayLength = productInfo.Count;
                response.ProductInfo = productInfo;

                return Ok(response);
            }
            else
            {
                response.Result = -96;
                response.msg = "ERD_VISMS_BILL_PARAMETER_ERROR";

                return BadRequest(response);
            }

        }

        [HttpPost ("purchase_gift")]
        public async Task<IActionResult> PurchaseGift([FromForm] string service_code, [FromForm] string ip, [FromForm] int reason, [FromForm] string sender_user_id, [FromForm] int sender_user_oid, [FromForm] int sender_user_age, [FromForm] string receiver_user_id, [FromForm] int receiver_user_oid ,[FromForm] int receiver_server_no, [FromForm] string message, [FromForm] string order_id, [FromForm] int payment_type, [FromForm] int payment_rule_id, [FromForm] int total_amount, [FromForm] int product_array_length, [FromForm] string product_info)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var validationResult = await ValidateRequest(service_code, ipAddress);
            if (validationResult != null)
                return validationResult;

            var response = new Response
            {
                service_code = service_code,
                Result = 1,
                msg = "",
                bonus_balance = null,
                real_balance = null,

            };

            if (string.IsNullOrEmpty(receiver_user_id))
            {
                response.Result = 12002;
                response.msg = "ERD_VISMS_BILL_RECIPIENT_NO_PLAYER";
                return BadRequest(response);
            }

            var receiver_user = await _context.VISMS_UserList
                            .FirstOrDefaultAsync(u => u.ServiceCode == service_code && u.strNexonID == receiver_user_id);
            if (receiver_user == null)
            {
                response.Result = 12003;
                response.msg = "ERD_VISMS_BILL_RECIPIENT_NO_PLAYER";
                return BadRequest(response);
            }

            if (string.IsNullOrEmpty(sender_user_id))
            {
                response.Result = 12003;
                response.msg = "ERD_VISMS_BILL_NO_PLAYER";
                return BadRequest(response);
            }

            var sender_user = await _context.VISMS_UserList
                .FirstOrDefaultAsync(u => u.ServiceCode == service_code && u.strNexonID == sender_user_id);
            if (sender_user == null)
            {
                response.Result = 12003;
                response.msg = "ERD_VISMS_BILL_NO_PLAYER";
                return BadRequest(response);
            }

            if (sender_user.RealBalance < total_amount)
            {
                response.Result = 12040;
                response.msg = "ERD_VISMS_BILL_INSUFFICIENT_BALANCE";
                return BadRequest(response);
            }
            var productInfo = JsonSerializer.Deserialize<List<VISMS_ProductInfo>>(product_info);
            if (productInfo != null && productInfo.Count > 0)
            {
                var productArray = productInfo.Select(p => new
                {
                    product_no = p.product_no,
                    order_quantity = p.order_quantity
                }).ToList();

                var purchaseLog = new VISMS_PurchaseLog
                {
                    ServiceCode = service_code,
                    OrderID = order_id,
                    ProductNo = productArray.First().product_no,
                    PaymentType = payment_type,
                    PaymentRuleID = payment_rule_id,
                    TotalPrice = total_amount,
                    OrderAmmount = productArray.First().order_quantity,
                    oid = sender_user_oid,
                    strNexonID = sender_user_id,
                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IsGift = true,
                    Receiver_oid = receiver_user_oid,
                    Receiver_strNexonID = receiver_user_id
                };

                var logId = await _context.VISMS_PurchaseLog.AddAsync(purchaseLog);
                await _context.SaveChangesAsync();

                // Atualize o saldo apenas se nenhuma outra falha ocorreu até agora.
                sender_user.RealBalance -= total_amount;
                _context.VISMS_UserList.Update(sender_user);
                await _context.SaveChangesAsync();

                response.Result = 1;
                response.msg = "ERD_VISMS_BILL_PAID_SUCCESS";
                response.OrderID = order_id;
                response.OrderNo = logId.Entity.SRL;
                response.PaymentRuleID = payment_rule_id;
                response.ProductArrayLength = productInfo.Count;
                response.ProductInfo = productInfo;

                return Ok(response);
            }
            else
            {
                response.Result = -96;
                response.msg = "ERD_VISMS_BILL_PARAMETER_ERROR";

                return BadRequest(response);
            }

        }

    }
}