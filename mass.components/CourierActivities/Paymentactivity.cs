using System;
using System.Net;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace mass.components.CourierActivities
{
    public class PaymentActivity : IActivity<PaymentArguments,PaymentLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<PaymentArguments> context)
        {
            string cardNumber = context.Arguments.CardNumber ?? throw new ArgumentNullException(nameof(cardNumber));
            await Task.Delay(5000);
            
            if (cardNumber.StartsWith("5999"))
            {
                throw new InvalidOperationException("The card number is invalid");
            }

            await Task.Delay(300);
            return context.Completed(new {AuthorizationCode = "77777"});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<PaymentLog> context)
        {
            await Task.Delay(100);
            return context.Compensated();
        }
    }
}