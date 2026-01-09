
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
        UVM4CsFunctionMode_NONE,

        /// <summary>
        /// Representation of a run that aims to upgrade the whole targeted project.
        /// </summary>
        WHOLE_PROJECT,

        /// <summary>
        /// Representation of a run that aims to upgrade only the necessary files to upgrade a given target in the targeted project.
        /// </summary>
        TARGET,

        /// <summary>
        /// SHOULD NOT BE ENCOUNTER! (Can be used to know the size of the enum.)
        /// </summary>
        UVM4CsFunctionMode_SIZE
    }
}
