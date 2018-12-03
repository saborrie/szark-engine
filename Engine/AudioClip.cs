/*
	AudioClip.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using NAudio.Wave;

namespace PGE
{
    public class AudioClip
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public AudioClip(string audioFileName)
        {
            if (Path.GetExtension(audioFileName) != ".wav")
            {
                Console.WriteLine("[ERROR]: Sound File Extension not supported!");
                return;
            }

            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                WaveFormat = audioFileReader.WaveFormat;

                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;

                while((samplesRead = audioFileReader.Read(readBuffer,0,readBuffer.Length)) > 0)
                    wholeFile.AddRange(readBuffer.Take(samplesRead));

                AudioData = wholeFile.ToArray();
            }
        }
    }
}