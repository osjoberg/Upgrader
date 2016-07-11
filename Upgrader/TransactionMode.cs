namespace Upgrader
{
    /// <summary>
    /// Transacton mode to use when performing a database schema update.
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// No transaction is used.
        /// </summary>
        None,

        /// <summary>
        /// One transaction is used for every step that is executed.
        /// </summary>
        OneTransactionPerStep
    }
}
