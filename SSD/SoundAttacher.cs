using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace SSD
{
    class SoundAttacher
    {

        public SoundAttacher(SoundEffectInstance soundEffectInstance, AudioListener listner, Entity entityToAttachTo, bool isLoop = false)
        {
            _emitter = new AudioEmitter();
            _listner = listner;
            _emitter.Position = entityToAttachTo.getMatrix().Translation;
            _soundInstance = soundEffectInstance;
            _soundInstance.Apply3D(listner, _emitter);
            _soundInstance.IsLooped = isLoop;

            _entityAttachedTo = entityToAttachTo;
            _soundInstance.Play();
        }

        public void update()
        {
            _emitter.Position = _entityAttachedTo.getMatrix().Translation;
            _soundInstance.Apply3D(_listner, _emitter);
        }

        SoundEffectInstance _soundInstance;
        AudioEmitter _emitter;
        AudioListener _listner;
        Entity _entityAttachedTo;
    }
}
