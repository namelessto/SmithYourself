using StardewValley;
using StardewValley.Objects;
using StardewValley.Buffs;

namespace MyBootsMod
{
    public class SwiftBoots : Boots
    {
        public SwiftBoots(string itemId) : base(itemId) { }

        public override void onEquip(Farmer who)
        {
            base.onEquip(who);

            var buff = new Buff("mybootsmod.swiftboots")
            {
                description = "+1 Speed from Swift Boots",
                millisecondsDuration = Buff.ENDLESS
            };
            buff.effects.Speed.Value += 1;
            who.applyBuff(buff);
        }

        public override void onUnequip(Farmer who)
        {
            base.onUnequip(who);
            who.buffs.Remove("mybootsmod.swiftboots");
        }
    }
}
