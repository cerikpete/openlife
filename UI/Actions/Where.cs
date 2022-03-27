﻿using Core;

namespace UI.Actions
{
    class Where : Action
    {
        public Where(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("swhere");
            usageString = parent.RM.GetString("uwhere");
        }

        public override void acceptInput(string verb, Parser args)
        {
            Primitive prim;
            Avatar avatar;
            base.acceptInput(verb, args);

            if (args.prepPhrases["is"].Length == 0)
            {
                parent.output("Provide something for which you need to know Where is it.");
            }
            else
            {
                if (((Listeners.Avatars)parent.listeners["avatars"]).tryGetAvatar(args.prepPhrases["is"], out avatar))
                {
                    //client.Self.Movement.Camera.AtAxis
                    LLVector3 myPos = client.Self.SimPosition;
                    LLVector3 forward = new LLVector3(1, 0, 0);
                    LLVector3 offset = LLVector3.Norm(avatar.Position - myPos);
                    LLQuaternion newRot2 = LLVector3.RotBetween(forward, offset);
                    
                    LLQuaternion newRot1 = LLVector3.RotBetween(avatar.Position, client.Self.RelativePosition);
                    double newDist = LLVector3.Dist(avatar.Position, client.Self.RelativePosition);
                    parent.output(client.Self.Movement.Camera.AtAxis + ", " + newRot2 + ", " + newDist);

                    //parent.output(avatar.Position.X + ", " + avatar.Position.Y + ", " + avatar.Position.Z);
                    //parent.output(client.Self.RelativePosition.X + ", " + client.Self.RelativePosition.Y + ", " + client.Self.RelativePosition.Z +"\n");

                    //parent.output(avatar.Rotation.X + ", " + avatar.Rotation.Y + ", " + avatar.Rotation.Z);
                    //parent.output(client.Self.RelativeRotation.X + ", " + client.Self.RelativeRotation.Y + ", " + client.Self.RelativeRotation.Z + "\n");
                }
                else if (((Listeners.Objects)parent.listeners["objects"]).tryGetPrim(args.prepPhrases["is"], out prim))
                {
                    LLQuaternion newRot = LLVector3.RotBetween(prim.Position, client.Self.RelativePosition);
                    double newDist = LLVector3.Dist(prim.Position, client.Self.RelativePosition);
                    parent.output(newRot + ", " + newDist);

                    //parent.output(prim.Position.X + ", " + prim.Position.Y + ", " + prim.Position.Z);
                    //parent.output(client.Self.RelativePosition.X + ", " + client.Self.RelativePosition.Y + ", " + client.Self.RelativePosition.Z + "\n");

                    //parent.output(prim.Rotation.X + ", " + prim.Rotation.Y + ", " + prim.Rotation.Z);
                    //parent.output(client.Self.RelativeRotation.X + ", " + client.Self.RelativeRotation.Y + ", " + client.Self.RelativeRotation.Z + "\n");
                }
                else
                {
                    parent.output("I don't know where is " + args.prepPhrases["is"] + ".");
                    return;
                }
            }
        }
    }
}
