public class VatCalculator
{
    public static decimal CalculateTotalWithVat(int quantity, decimal price, decimal vatRate)
        => quantity * price * (1 + vatRate);
}
