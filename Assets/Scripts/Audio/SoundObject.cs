using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

namespace GD3D.Audio
{
    /// <summary>
    /// A object that plays sound. Is used as the pool object in the <see cref="SoundManager"/>
    /// </summary>
    public class SoundObject : PoolObject
    {
        private AudioSource source;
        public AudioSource GetSource => source;

        public override void OnCreated()
        {
            base.OnCreated();

            // Get component
            source = GetComponent<AudioSource>();
        }

        public void Play()
        {
            // Play the sound
            source.Play();

            RemoveAfterTime(source.clip.length);
        }
    }
}
