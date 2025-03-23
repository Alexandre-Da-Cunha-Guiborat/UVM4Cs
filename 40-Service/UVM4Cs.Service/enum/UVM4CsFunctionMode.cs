
namespace UVM4Cs.Service
{
    /// <summary>
    /// Enum used to specify the type of run to do.
    /// </summary>
    public enum UVM4CsFunctionMode
    {
        /// <summary>
        /// SHOULD NOT BE ENCOUNTER! (Can be used for initialization and error detection.)
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Reprensetation of a run that aims to upgrade the whole targeted project.
        /// </summary>
        WHOLEPROJECT = 1,

        /// <summary>
        /// Reprensetation of a run that aims to upgrade only the necessary files to upgrade a given target in the targeted project.
        /// </summary>
        TARGET = 2
    }
}
