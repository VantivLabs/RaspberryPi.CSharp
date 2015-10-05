using System;
using System.Threading.Tasks;

namespace RaspberryPi.CSharp
{
    public class MercuryAccess
    {
        public async Task<bool> CallMPS()
        {
            var rnd = new Random();
            var amount = rnd.NextDouble() * 10;
             
            var xml = @"<?xml version=""1.0""?>
                            <TStream>
                                  <Transaction>
                                  <MerchantID>003503902913105</MerchantID>
                                  <OperatorID>test</OperatorID>
                                  <TranType>Credit</TranType>
                                  <TranCode>Sale</TranCode>
                                  <Memo>dano raspberry pi csharp</Memo>
                                     <InvoiceNo>123456</InvoiceNo>
                                     <RefNo>123456</RefNo>
                                     <Amount>
                                         <Purchase>{0}</Purchase>
                                     </Amount>
                                     <Account>
                                   <AcctNo>5499990123456781</AcctNo>
                                   <ExpDate>0816</ExpDate>
                                   <AccountSource>Keyed</AccountSource>
                                     </Account>
                                     </Transaction>
                                   </TStream>";

            xml = String.Format(xml, Math.Round(amount,2));


            var client = new MPSWebService.wsSoapClient();
            
            var response = await client.CreditTransactionAsync(xml, "xyz").ConfigureAwait(false);

            var posCmdStatus = response.IndexOf("<CmdStatus>");
            var posEndCmdStatus = response.IndexOf("</CmdStatus>");
            var cmdStatus = response.Substring(posCmdStatus + 11, (posEndCmdStatus - posCmdStatus - 11));

            if (cmdStatus.ToLower() == "approved")
            {
                return true;
            }

            return false;
            
        }
    }
}
