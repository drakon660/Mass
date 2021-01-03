using mass.contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mass.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;

        [HttpPost]
        public async Task<IActionResult> Post(object model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = model.Id,
                InVar.Timestamp,
                model.CustomerNumber,
                model.PaymentCardNumber,
                model.Notes
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;

                return Accepted(response);
            }

            if (accepted.IsCompleted)
            {
                await accepted;

                return Problem("Order was not accepted");
            }
            else
            {
                var response = await rejected;

                return BadRequest(response.Message);
            }
        }
    }
}
