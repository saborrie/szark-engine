/*
	Audio.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using System;
using System.IO;
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

        private void AddMixerInput(ISampleProvider input) =>
            mixer.AddMixerInput(ConvertToRightChannelCount(input));

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
                return input;
            else
                return new MonoToStereoSampleProvider(input);
        }

        /// <summary>
        /// Play Sound from File
        /// </summary>
        /// <param name="fileName">File Path / Name</param>
        /// <param name="volume">Audio Volume</param>
        public void PlaySound(string fileName, float volume = 1)
        {
            if (Path.GetExtension(fileName) != ".wav")
            {
                Console.WriteLine("[ERROR]: Sound File Extension not supported!");
                return;
            }

            outputDevice.Volume = volume;
            AddMixerInput(new AudioReader(new AudioFileReader(fileName)));
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
    }
}