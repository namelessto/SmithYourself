using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmithYourself;
using StardewValley.Extensions;
using static StardewValley.FarmerSprite;

namespace StardewValley.Menus;

public class PlayerGeodeMenu : MenuWithInventory
{
    public const int region_geodeSpot = 998;
    public ClickableComponent geodeSpot;
    public AnimatedSprite playerSprite;
    public TemporaryAnimatedSprite geodeDestructionAnimation;
    public TemporaryAnimatedSprite sparkle;
    public int geodeAnimationTimer;
    public int yPositionOfGem;
    public int alertTimer;
    public float delayBeforeShowArtifactTimer;
    public Item geodeTreasure;
    public Item geodeTreasureOverride;
    public bool waitingForServerResponse;
    private TemporaryAnimatedSpriteList fluffSprites = new();
    public static Farmer farmer;
    public bool isUsingTool = false;
    public string description;

    public PlayerGeodeMenu(string menuDescription)
        : base(null, okButton: true, trashCan: true, 12, 132)
    {
        if (yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        {
            movePosition(0, -IClickableMenu.spaceToClearTopBorder);
        }
        description = menuDescription;
        inventory.highlightMethod = HighlightGeodes;
        geodeSpot = new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "")
        {
            myID = 998,
            downNeighborID = 0
        };
        farmer = Game1.player.CreateFakeEventFarmer();
        farmer.faceDirection(3);
        // Tool pickaxe = ItemRegistry.Create<Tool>("Pickaxe", 1);
        // farmer.CurrentTool = pickaxe;
        playerSprite = new AnimatedSprite();
        playerSprite.SetOwner(farmer);
        playerSprite.faceDirection(3);
        playerSprite.CurrentFrame = 6;
        List<ClickableComponent> list = inventory.inventory;
        if (list != null && list.Count >= 12)
        {
            for (int i = 0; i < 12; i++)
            {
                if (inventory.inventory[i] != null)
                {
                    inventory.inventory[i].upNeighborID = 998;
                }
            }
        }
        if (trashCan != null)
        {
            trashCan.myID = 106;
        }

        if (okButton != null)
        {
            okButton.leftNeighborID = 11;
        }

        if (Game1.options.SnappyMenus)
        {
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }
    }

    public override void snapToDefaultClickableComponent()
    {
        currentlySnappedComponent = getComponentWithID(0);
        snapCursorToCurrentSnappedComponent();
    }

    public override bool readyToClose()
    {
        if (base.readyToClose() && geodeAnimationTimer <= 0 && base.heldItem == null)
        {
            return !waitingForServerResponse;
        }

        return false;
    }

    public bool HighlightGeodes(Item i)
    {
        if (base.heldItem == null)
        {
            try
            {
                return Utility.IsGeode(i) && ModEntry.Config.GeodeAllowances[ToolType.Geode][i.ItemId];
            }
            catch
            {
                return Utility.IsGeode(i);
            }
        }

        return true;
    }

    public virtual void StartGeodeCrack()
    {
        geodeSpot.item = base.heldItem.getOne();
        base.heldItem = base.heldItem.ConsumeStack(1);
        geodeAnimationTimer = 500;
        Game1.playSound("stoneStep");
        playerSprite.SetOwner(farmer);
        List<AnimationFrame> frames = new()
        {
            new AnimationFrame(48, 50, secondaryArm: false, flip: true),
            new AnimationFrame(49, 50, secondaryArm: false, flip: true , behaviorAtEndOfFrame: false),
            new AnimationFrame(50, 50, secondaryArm: false, flip: true,behaviorAtEndOfFrame: false),
            new AnimationFrame(51, 50, secondaryArm: false, flip: true),
            new AnimationFrame(52, 50, secondaryArm: false, flip: true, behaviorAtEndOfFrame: false)
        };
        playerSprite.setCurrentAnimation(frames);
        playerSprite.loop = false;
    }
    
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (waitingForServerResponse)
        {
            return;
        }

        base.receiveLeftClick(x, y, playSound: true);
        if (!geodeSpot.containsPoint(x, y))
        {
            return;
        }

        if (base.heldItem != null && Utility.IsGeode(base.heldItem) && geodeAnimationTimer <= 0)
        {
            int num = Game1.player.freeSpotsInInventory();
            if (num > 1 || (num == 1 && base.heldItem.Stack == 1))
            {
                if (base.heldItem.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                {
                    waitingForServerResponse = true;
                    Game1.player.team.goldenCoconutMutex.RequestLock(delegate
                    {
                        waitingForServerResponse = false;
                        geodeTreasureOverride = ItemRegistry.Create("(O)73");
                        StartGeodeCrack();
                    }, delegate
                    {
                        waitingForServerResponse = false;
                        StartGeodeCrack();
                    });
                }
                else
                {
                    StartGeodeCrack();
                }
            }
            else
            {
                descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
                wiggleWordsTimer = 500;
                alertTimer = 1500;
            }
        }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        base.receiveRightClick(x, y, playSound: true);
    }

    public override void performHoverAction(int x, int y)
    {
        if (alertTimer > 0)
        {
            return;
        }

        base.performHoverAction(x, y);
        if (descriptionText.Equals(""))
        {

            descriptionText = description;
        }
    }

    public override void emergencyShutDown()
    {
        base.emergencyShutDown();
        if (base.heldItem != null)
        {
            Game1.player.addItemToInventoryBool(base.heldItem);
        }
    }

    public override void update(GameTime time)
    {
        base.update(time);
        fluffSprites.RemoveWhere((TemporaryAnimatedSprite sprite) => sprite.update(time));
        if (alertTimer > 0)
        {
            alertTimer -= time.ElapsedGameTime.Milliseconds;
        }

        if (geodeAnimationTimer <= 0)
        {
            return;
        }

        Game1.MusicDuckTimer = 0;
        geodeAnimationTimer -= time.ElapsedGameTime.Milliseconds;
        if (geodeAnimationTimer <= 0)
        {
            geodeDestructionAnimation = null;
            geodeSpot.item = null;
            if (geodeTreasure?.QualifiedItemId == "(O)73")
            {
                Game1.netWorldState.Value.GoldenCoconutCracked = true;
            }

            Game1.player.addItemToInventoryBool(geodeTreasure);
            geodeTreasure = null;
            yPositionOfGem = 0;
            fluffSprites.Clear();
            delayBeforeShowArtifactTimer = 0f;
            return;
        }
        int currentFrame = playerSprite.CurrentFrame;
        playerSprite.animateOnce(time);

        if (playerSprite.currentFrame == 50 && currentFrame != 50)
        {
            if (geodeSpot.item?.QualifiedItemId == "(O)275" || geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
            {
                Game1.playSound("hammer");
                Game1.playSound("woodWhack");
            }
            else
            {
                Game1.playSound("hammer");
                Game1.playSound("stoneCrack");
            }

            Game1.stats.GeodesCracked++;
            if (geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
            {
                Game1.stats.Increment("MysteryBoxesOpened");
            }

            int num = 448;
            if (geodeSpot.item != null)
            {
                string qualifiedItemId = geodeSpot.item.QualifiedItemId;
                if (!(qualifiedItemId == "(O)536"))
                {
                    if (qualifiedItemId == "(O)537")
                    {
                        num += 128;
                    }
                }
                else
                {
                    num += 64;
                }

                geodeDestructionAnimation = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, num, 64, 64), 100f, 8, 0, new Vector2(geodeSpot.bounds.X + 392 - 32, geodeSpot.bounds.Y + 192 - 32), flicker: false, flipped: false);
                switch (geodeSpot.item?.QualifiedItemId)
                {
                    case "(O)275":
                        {
                            geodeDestructionAnimation = new TemporaryAnimatedSprite
                            {
                                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                                sourceRect = new Rectangle(388, 123, 18, 21),
                                sourceRectStartingPos = new Vector2(388f, 123f),
                                animationLength = 6,
                                position = new Vector2(geodeSpot.bounds.X + 380 - 32, geodeSpot.bounds.Y + 192 - 32),
                                holdLastFrame = true,
                                interval = 50f,
                                id = 777,
                                scale = 4f
                            };
                            for (int j = 0; j < 6; j++)
                            {
                                fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21), geodeSpot.bounds.Y + 192 - 16), flipped: false, 0.002f, new Color(255, 222, 198))
                                {
                                    alphaFade = 0.02f,
                                    motion = new Vector2((float)Game1.random.Next(-20, 21) / 10f, (float)Game1.random.Next(5, 20) / 10f),
                                    interval = 99999f,
                                    layerDepth = 0.9f,
                                    scale = 3f,
                                    scaleChange = 0.01f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * MathF.PI / 256f,
                                    delayBeforeAnimationStart = j * 20
                                });
                                fluffSprites.Add(new TemporaryAnimatedSprite
                                {
                                    texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                                    sourceRect = new Rectangle(499, 132, 5, 5),
                                    sourceRectStartingPos = new Vector2(499f, 132f),
                                    motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-7, -4)),
                                    acceleration = new Vector2(0f, 0.25f),
                                    totalNumberOfLoops = 1,
                                    interval = 30f,
                                    alphaFade = 0.015f,
                                    animationLength = 1,
                                    layerDepth = 1f,
                                    scale = 4f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * MathF.PI / 256f,
                                    delayBeforeAnimationStart = j * 10,
                                    position = new Vector2(geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21), geodeSpot.bounds.Y + 192 - 16)
                                });
                                delayBeforeShowArtifactTimer = 10f;
                            }

                            break;
                        }
                    case "(O)MysteryBox":
                    case "(O)GoldenMysteryBox":
                        {
                            geodeDestructionAnimation = new TemporaryAnimatedSprite
                            {
                                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors_1_6"),
                                sourceRect = new Rectangle((geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox") ? 256 : 0, 27, 24, 24),
                                sourceRectStartingPos = new Vector2((geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox") ? 256 : 0, 27f),
                                animationLength = 8,
                                position = new Vector2(geodeSpot.bounds.X + 380 - 48, geodeSpot.bounds.Y + 192 - 48),
                                holdLastFrame = true,
                                interval = 30f,
                                id = 777,
                                scale = 4f
                            };
                            for (int i = 0; i < 6; i++)
                            {
                                fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(geodeSpot.bounds.X + 392 - 48 + Game1.random.Next(32), geodeSpot.bounds.Y + 192 - 24), flipped: false, 0.002f, new Color(255, 222, 198))
                                {
                                    alphaFade = 0.02f,
                                    motion = new Vector2((float)Game1.random.Next(-20, 21) / 10f, (float)Game1.random.Next(5, 20) / 10f),
                                    interval = 99999f,
                                    layerDepth = 0.9f,
                                    scale = 3f,
                                    scaleChange = 0.01f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * MathF.PI / 256f,
                                    delayBeforeAnimationStart = i * 20
                                });
                                int num2 = Game1.random.Next(3);
                                fluffSprites.Add(new TemporaryAnimatedSprite
                                {
                                    texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors_1_6"),
                                    sourceRect = new Rectangle(((geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox") ? 15 : 0) + num2 * 5, 52, 5, 5),
                                    sourceRectStartingPos = new Vector2(num2 * 5, 75f),
                                    motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-7, -4)),
                                    acceleration = new Vector2(0f, 0.25f),
                                    totalNumberOfLoops = 1,
                                    interval = 30f,
                                    alphaFade = 0.015f,
                                    animationLength = 1,
                                    layerDepth = 1f,
                                    scale = 4f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * MathF.PI / 256f,
                                    delayBeforeAnimationStart = i * 10,
                                    position = new Vector2(geodeSpot.bounds.X + 392 - 48 + Game1.random.Next(32), geodeSpot.bounds.Y + 192 - 24)
                                });
                                delayBeforeShowArtifactTimer = 10f;
                            }

                            break;
                        }
                }

                if (geodeTreasureOverride != null)
                {
                    geodeTreasure = geodeTreasureOverride;
                    geodeTreasureOverride = null;
                }
                else
                {
                    geodeTreasure = Utility.getTreasureFromGeode(geodeSpot.item);
                }

                if (!(geodeSpot.item.QualifiedItemId == "(O)275") && (!(geodeTreasure is Object @object) || !(@object.Type == "Minerals")) && geodeTreasure is Object object2 && object2.Type == "Arch" && !Game1.player.hasOrWillReceiveMail("artifactFound"))
                {
                    geodeTreasure = ItemRegistry.Create("(O)390", 5);
                }
            }
        }

        if (geodeDestructionAnimation != null && ((geodeDestructionAnimation.id != 777 && geodeDestructionAnimation.currentParentTileIndex < 7) || (geodeDestructionAnimation.id == 777 && geodeDestructionAnimation.currentParentTileIndex < 5)))
        {
            geodeDestructionAnimation.update(time);
            if (delayBeforeShowArtifactTimer > 0f)
            {
                delayBeforeShowArtifactTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
                if (delayBeforeShowArtifactTimer <= 0f)
                {
                    fluffSprites.Add(geodeDestructionAnimation);
                    fluffSprites.Reverse();
                    geodeDestructionAnimation = new TemporaryAnimatedSprite
                    {
                        interval = 60f,
                        animationLength = 6,
                        alpha = 0.001f,
                        id = 777
                    };
                }
            }
            else
            {
                if (geodeDestructionAnimation.currentParentTileIndex < 3)
                {
                    yPositionOfGem--;
                }

                yPositionOfGem--;
                if (geodeDestructionAnimation.currentParentTileIndex == 7 || (geodeDestructionAnimation.id == 777 && geodeDestructionAnimation.currentParentTileIndex == 5))
                {
                    if (!(geodeTreasure is Object object3) || object3.price.Value > 75 || geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                    {
                        if (geodeSpot.item != null)
                        {
                            sparkle = new TemporaryAnimatedSprite(
                                "TileSheets\\animations",
                                new Rectangle(0, 640, 64, 64),
                                10f,
                                8,
                                0,
                                new Vector2(geodeSpot.bounds.X + ((geodeSpot.item.itemId.Value == "MysteryBox") ? 94 : 98) * 4 - 32,
                                geodeSpot.bounds.Y + 192 + yPositionOfGem - 32),
                                flicker: false, flipped: false
                            );
                        }

                        Game1.playSound("discoverMineral");
                    }
                    else
                    {
                        Game1.playSound("newArtifact");
                    }
                }
            }
        }

        if (sparkle != null && sparkle.update(time))
        {
            sparkle = null;
        }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
        Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
        xPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.X;
        yPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.Y;
        Item item = geodeSpot.item;
        geodeSpot = new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "Anvil")
        {
            item = item
        };
        int yPosition = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16 + 128 + 4;
        if (okButton != null)
        {
            okButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - 192 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 4857,
                upNeighborID = 5948,
                leftNeighborID = 12
            };
        }

        if (trashCan != null)
        {
            trashCan = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - 192 - 32 - IClickableMenu.borderWidth - 104, 64, 104), Game1.mouseCursors, new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26), 4f)
            {
                myID = 5948,
                downNeighborID = 4857,
                leftNeighborID = 12,
                upNeighborID = 106
            };
        }

        inventory = new InventoryMenu(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPosition, playerInventory: false, null, inventory.highlightMethod);
    }

    public override void draw(SpriteBatch b)
    {
        if (!Game1.options.showClearBackgrounds)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
        }

        base.draw(b);

        b.Draw(Game1.mouseCursors, new Vector2(geodeSpot.bounds.X, geodeSpot.bounds.Y), new Rectangle(0, 512, 140, 78), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        if (geodeSpot.item != null)
        {
            if (geodeDestructionAnimation == null)
            {
                Vector2 vector = Vector2.Zero;
                if (geodeSpot.item.QualifiedItemId == "(O)275")
                {
                    vector = new Vector2(-2f, 2f);
                }
                else if (geodeSpot.item.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                {
                    vector = new Vector2(-7f, 4f);
                }

                _ = geodeSpot.item.QualifiedItemId == "(O)275";
                geodeSpot.item.drawInMenu(b, new Vector2(geodeSpot.bounds.X + 360, geodeSpot.bounds.Y + 160) + vector, 1f);
            }
            else
            {
                geodeDestructionAnimation.draw(b, localPosition: true);
            }

            foreach (TemporaryAnimatedSprite fluffSprite in fluffSprites)
            {
                fluffSprite.draw(b, localPosition: true);
            }

            if (geodeTreasure != null && delayBeforeShowArtifactTimer <= 0f)
            {
                geodeTreasure.drawInMenu(b, new Vector2(geodeSpot.bounds.X + (geodeSpot.item.QualifiedItemId.Contains("MysteryBox") ? 86 : 90) * 4, geodeSpot.bounds.Y + 160 + yPositionOfGem), 1f);
            }

            sparkle?.draw(b, localPosition: true);
        }

        farmer.FarmerRenderer.draw(
            b,
            farmer,
            playerSprite.CurrentFrame,
            new Vector2(geodeSpot.bounds.X + 430, geodeSpot.bounds.Y + 128),
            1f,
            flip: true
        );

        if (!hoverText.Equals(""))
        {
            IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
        }
        base.heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        if (!Game1.options.hardwareCursor)
        {
            drawMouse(b);
        }
    }
}