/*
	Audio.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace PGE
{
    public class Audio
    {
        private IWavePlayer outputDevice;
        private MixingSampleProvider mixer;

        public Audio()
        {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
        }

        private void AddMixerInput(ISampleProvider input)
        {
            if (input.WaveFormat.Channels != mixer.WaveFormat.Channels)
                input = new MonoToStereoSampleProvider(input);
            mixer.AddMixerInput(input);
        }

        /// <summary>
        /// Play Sound from File
        /// </summary>
        /// <param name="filePath">File Path / Name</param>
        /// <param name="volume">Audio Volume</param>
        public void PlaySound(string filePath, float volume = 1)
        {
            if (!IsSupportedAudio(filePath))
                return;
            
            outputDevice.Volume = volume;
            AddMixerInput(new AudioReader(new AudioFileReader(filePath)));
            outputDevice.Play();
        }

        /// <summary>
        /// Play Sound from Cached Sound Object
        /// </summary>
        /// <param name="sound">Sound</param>
        /// <param name="volume">Volume</param>
        public void PlaySound(AudioClip sound, float volume = 1)
        {
            if (sound == null) return;

            outputDevice.Volume = volume;
            AddMixerInput(new SoundSample(sound));
            outputDevice.Play();
        }

        /// <summary>
        /// Checks if the audio file extension at the path is supported.
        /// Also writes to console if file is not supported.
        /// </summary>
        /// <param name="filePath">The path of the audio file</param>
        /// <returns>If extension is supported</returns>
        public static bool IsSupportedAudio(string filePath)
        {
            if (Path.GetExtension(filePath) != ".wav")
            {
                Debug.Log($"Sound file at [{filePath}] - extension not supported!", 
                    LogLevel.ERROR);
                return false;
            }

            return true;
        }
    }

    public class AudioClip
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public AudioClip(string filePath)
        {
            if (!Audio.IsSupportedAudio(filePath))
                return;

            using (var audioFileReader = new AudioFileReader(filePath))
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

    class AudioReader : ISampleProvider
    {
        public WaveFormat WaveFormat { get; private set; }

        private bool isDisposed;
        private readonly AudioFileReader reader;

        public AudioReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed) return 0;
            
            int read = reader.Read(buffer, offset, count);

            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }

            return read;
        }
    }

    class SoundSample : ISampleProvider
    {
        private readonly AudioClip cachedSound;
        private long position;

        public SoundSample(AudioClip cachedSound) {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }
}