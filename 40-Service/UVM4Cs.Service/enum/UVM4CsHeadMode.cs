
namespace UVM4Cs.Service
{
    /// <summary>
    /// Enum used to specify the commit to compare too.
    /// </summary>
    public enum UVM4CsHeadMode
    {
        /// <summary>
        /// SHOULD NOT BE ENCOUNTER! (Can be used for initialization and error detection.)
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Reprensetation of a comparaison with the branchRef/head
        /// </summary>
        HEAD = 1,

        /// <summary>
        /// Reprensetation of a comparaison with the branchRef/prevCommit
        /// </summary>
        PREVCOMMIT = 2
    }
}
