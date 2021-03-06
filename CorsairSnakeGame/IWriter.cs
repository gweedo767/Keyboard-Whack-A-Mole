﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the IWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace KeybaordAudio
{
    public interface IWriter
    {
        void Write(int iter, byte[] fftData);
    }
}
