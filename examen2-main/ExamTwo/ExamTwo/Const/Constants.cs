namespace ExamTwo.Const
{
    public static class Constants
    {
        // Numeric Constants
        public const int ZERO = 0;
        public const int MIN_QUANTITY = 0;


        // Error Messages - Service
        public static class ServiceErrorMessages
        {
            public const string ORDER_EMPTY = "Orden vacía.";
            public const string QUANTITY_MUST_BE_GREATER_THAN_ZERO = "La cantidad de {0} debe ser mayor a cero.";
            public const string COFFEE_TYPE_NOT_EXISTS = "El tipo de café {0} no existe.";
            public const string INSUFFICIENT_COFFEE_STOCK = "No hay suficientes {0} en la máquina.";
            public const string INVALID_PAYMENT_DENOMINATIONS = "El pago contiene denominaciones inválidas.";
            public const string INSUFFICIENT_MONEY = "Dinero insuficiente.";
            public const string INSUFFICIENT_CHANGE = "Fallo al realizar la compra. No hay suficiente cambio en la máquina.";
        }

        // Success Messages
        public static class SuccessMessages
        {
            public const string PURCHASE_SUCCESS = "Compra realizada exitosamente.";
        }

        // Change Breakdown Messages
        public static class ChangeMessages
        {
            public const string CHANGE_AMOUNT = "Su vuelto es de {0} colones.";
            public const string BREAKDOWN_HEADER = "Desglose:";
            public const string COIN_SINGULAR = "moneda";
            public const string COIN_PLURAL = "monedas";
            public const string COIN_FORMAT = "{0} {1} de {2}";
        }
    }
}

