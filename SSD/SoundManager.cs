using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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

            _audioEngine = new AudioEngine("Content/XACTProject.xgs");
            _bulletWaveBank = new WaveBank(_audioEngine, "Content/BulletWaveBank.xwb");
            _bulletSoundBank = new SoundBank(_audioEngine, "Content/BulletSoundBank.xsb");
        }

        public void addAttatchment(LoadedSounds soundEnum, Entity entityToAttachTo)
        {
            _soundAttatchments.Add(new SoundAttacher(_bulletSoundBank.GetCue("Missile"), _listner, entityToAttachTo));
        }

        public void update()
        {
            _audioEngine.Update();

            //Update the listner position to be that of the player
            float backupYaw = _player.getYaw();
            _player.setYaw(90);
            Matrix playerMatrix = _player.getMatrix();
            _listner.Forward = playerMatrix.Forward;
            _listner.Up = playerMatrix.Up;
            _listner.Position = playerMatrix.Translation;
            _player.setYaw(MathHelper.ToDegrees(backupYaw));

            //Remove any sounds that are marked as not being played.
            _soundAttatchments.RemoveAll(delegate(SoundAttacher e)
                {
                    return !e.isPlaying();
                }
            );  

            //Run update on all the sounds
            foreach(SoundAttacher attatchedSound in _soundAttatchments){
                attatchedSound.update();
            }
        }

        AudioEngine _audioEngine;
        WaveBank _bulletWaveBank;
        SoundBank _bulletSoundBank;

        ContentManager _contentManager;
        List<SoundAttacher> _soundAttatchments;
        List<SoundEffect> _soundEffects;
        AudioListener _listner;
        PlayerEntity _player;
    }
}
