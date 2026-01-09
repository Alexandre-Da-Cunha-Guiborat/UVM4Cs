
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
        UVM4CsHeadMode_NONE,

        /// <summary>
        /// Representation of a comparison with the branchRef/head
        /// </summary>
        HEAD,

        /// <summary>
        /// Representation of a comparison with the branchRef/prevCommit
        /// </summary>
        PREVIOUS_COMMIT,

        /// <summary>
        /// SHOULD NOT BE ENCOUNTER! (Can be used to know the size of the enum.)
        /// </summary>
        UVM4CsHeadMode_SIZE
    }
}
