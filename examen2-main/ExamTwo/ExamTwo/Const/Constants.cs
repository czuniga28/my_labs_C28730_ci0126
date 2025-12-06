namespace ExamTwo.Const
{
    public static class Constants
    {
        // HTTP Status Codes
        public const int HTTP_STATUS_INTERNAL_SERVER_ERROR = 500;

        // Numeric Constants
        public const int ZERO = 0;
        public const int MIN_QUANTITY = 0;

        // Error Messages - Controller
        public static class ErrorMessages
        {
            public const string ERROR_GET_COFFEES = "Error al obtener los cafés disponibles: {0}";
            public const string ERROR_CALCULATE_TOTAL = "Error al calcular el total: {0}";
            public const string ERROR_PROCESS_PURCHASE = "Error al procesar la compra: {0}";
            public const string REQUEST_NULL = "La solicitud no puede estar vacía.";
            public const string ORDER_EMPTY = "La orden no puede estar vacía.";
            public const string PAYMENT_NULL = "El pago no puede estar vacío.";
        }

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

