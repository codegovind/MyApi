namespace TaxAccount.Helpers
{
    public static class GstCalculator
    {
        // Returns true if IGST applies (inter-state)
        // Returns false if CGST+SGST applies (intra-state)
        public static bool IsInterState(
            string? tenantState, string? contactState)
        {
            if (string.IsNullOrEmpty(tenantState) ||
                string.IsNullOrEmpty(contactState))
                return false;

            return !tenantState.Equals(
                contactState, StringComparison.OrdinalIgnoreCase);
        }

        public static (decimal cgst, decimal sgst, decimal igst)
            CalculateGst(decimal amount, decimal gstPercent, bool isInterState)
        {
            if (isInterState)
            {
                return (0, 0, Math.Round(amount * gstPercent / 100, 2));
            }
            else
            {
                var halfRate = gstPercent / 2;
                var cgst = Math.Round(amount * halfRate / 100, 2);
                var sgst = Math.Round(amount * halfRate / 100, 2);
                return (cgst, sgst, 0);
            }
        }
    }
}