using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SSD
{
    public enum LoadedSounds { ROCKET_SOUND, PLASMA_SOUND, NORMAL_SOUND };
    class SoundManager
    {
        public SoundManager(ContentManager contentManager, PlayerEntity player)
        {
            _contentManager = contentManager;
            _soundAttatchments = new List<SoundAttacher>();
            _soundEffects = new List<SoundEffect>();

            _soundEffects.Add(_contentManager.Load<SoundEffect>("missile_sound"));

            _listner = new AudioListener();
            _player = player;
            _listner.Position = _player.getMatrix().Translation;
        }

        public void addAttatchment(LoadedSounds soundEnum, Entity entityToAttachTo)
        {
            
            _soundAttatchments.Add(new SoundAttacher(_soundEffects[(int)soundEnum].CreateInstance(), _listner, entityToAttachTo));
        }

        public void update()
        {
            _listner.Position = _player.getMatrix().Translation;

            foreach(SoundAttacher attatchedSound in _soundAttatchments){
                attatchedSound.update();
            }

        }

        ContentManager _contentManager;
        List<SoundAttacher> _soundAttatchments;
        List<SoundEffect> _soundEffects;
        AudioListener _listner;
        PlayerEntity _player;
    }
}
