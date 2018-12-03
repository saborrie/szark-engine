/*
	SoundSample.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using System;
using NAudio.Wave;

namespace PGE
{
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