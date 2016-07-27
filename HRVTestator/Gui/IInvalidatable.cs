namespace HRVTestator.Gui
{
    /// <summary>
    /// Das Interface <see cref="IInvalidatable"/> beitet die Möglichkeit eine View als Invalid zu flagen.
    /// </summary>
    public interface IInvalidatable
    {
        /// <summary>
        /// Invalidatet die Instanz.
        /// </summary>
        void Invalidate();
    }
}