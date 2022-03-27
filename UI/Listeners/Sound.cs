using Core;

namespace UI.Listeners
{
    class Sound : Listener
    {
        public Sound(TextForm parent)
            : base(parent)
        {
            client.Sound.OnPreloadSound += new SoundManager.PreloadSoundCallback(Sound_OnPreloadSound);
            client.Sound.OnSoundTrigger += new SoundManager.SoundTriggerCallback(Sound_OnSoundTrigger);
        }

        void Sound_OnSoundTrigger(LLUUID soundID, LLUUID ownerID, LLUUID objectID, LLUUID parentID, 
            float gain, ulong regionHandle, LLVector3 position)
        {
            //parent.output("sound trigger " + soundID);
        }

        void Sound_OnPreloadSound(LLUUID soundID, LLUUID ownerID, LLUUID objectID)
        {
            //parent.output("preload sound " + soundID);
        }
    }
}
