/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Katarina Kostkoa
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GhostlyLib.Animations
{
    public class ImagesAndAnimations
    {
        #region Private members

        private static ImagesAndAnimations _instance;

        private Texture2D _gameCharacterImg1, _gameCharacterImg2, _gameCharacterImg3,
            _gameCharacterImg4, _gameCharacterImg5, _gameCharacterImg6, _gameCharacterImg7,
            _gameCharacterImg8, _gameCharacterImg9, _gameCharacterImg10, _gameCharacterImg11;
        private Texture2D _characterHitImg3, _characterHitImg4, _characterHitImg5;

        private Texture2D _swimmingGameCharacter1, _swimmingGameCharacter2, _swimmingGameCharacter3;
        private Texture2D _swimmingCharacterHit1, _swimmingCharacterHit2, _swimmingCharacterHit3;

        private Texture2D _redEnemyFullLifeImg4, _redEnemyFullLifeImg5;
        private Texture2D _redEnemyHalfLifeImg4, _redEnemyHalfLifeImg5;
        private Texture2D _redEnemyLittleLifeImg4, _redEnemyLittleLifeImg5;

        private Texture2D _blackEnemyFullLifeImg4, _blackEnemyFullLifeImg5;
        private Texture2D _blackEnemyHalfLifeImg4, _blackEnemyHalfLifeImg5;
        private Texture2D _blackEnemyLittleLifeImg4, _blackEnemyLittleLifeImg5;

        private Texture2D _yellowEnemyFullLifeImg4, _yellowEnemyFullLifeImg5;

        private Texture2D _blackFishSwim1, _blackFishSwim2;
        private Texture2D _redFishSwim1, _redFishSwim2;
        private Texture2D _yellowFishSwim1, _yellowFishSwim2;
        private Texture2D _greenFishSwim1, _greenFishSwim2;

        private Texture2D _blackFly1, _blackFly2;
        private Texture2D _redFly1, _redFly2;
        private Texture2D _yellowFly1, _yellowFly2;
        private Texture2D _greenFly1, _greenFly2;

        private Texture2D _plusOne01, _plusOne02, _plusOne03;
        private Texture2D _plusTwo01, _plusTwo02, _plusTwo03;
        private Texture2D _plusThree01, _plusThree02, _plusThree03;

        private Texture2D _minusOne01, _minusOne02, _minusOne03;
        private Texture2D _minusThree01, _minusThree02, _minusThree03;

        private Texture2D _fireball01, _fireball02, _fireball03, _fireball04;
        private Texture2D _projectile01, _projectile02, _projectile03;

        private Texture2D _coin_bronze01, _coin_bronze02, _coin_bronze03, _coin_bronze04, _coin_bronze05;
        private Texture2D _coin_silver01, _coin_silver02, _coin_silver03, _coin_silver04, _coin_silver05;
        private Texture2D _coin_gold01, _coin_gold02, _coin_gold03, _coin_gold04, _coin_gold05;

        #endregion Private members

        #region Public member
        public static ImagesAndAnimations Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ImagesAndAnimations();
                }
                return _instance;
            }
        }

        public Texture2D CharacterJumping { get; private set; }
        public Texture2D Background { get; set; }
        public Texture2D BackgroundClosest { get; set; }
        public Texture2D BackgroundCloser { get; set; }
        public Texture2D BackgroundClose { get; set; }
        public Texture2D BackgroundClosestWater { get; set; }
        public Texture2D BackgroundCloserWater { get; set; }
        public Texture2D BackgroundCloseWater { get; set; }
        public Texture2D BackgroundClosestRock { get; set; }
        public Texture2D BackgroundCloserRock { get; set; }
        public Texture2D BackgroundCloseRock { get; set; }
        public Texture2D BackgroundFar { get; set; }
        public Texture2D BackgroundFurther { get; set; }
        public Texture2D BackgroundFurthest { get; set; }
        public Texture2D BackgroundWater { get; set; }
        public Texture2D Crate { get; private set; }
        public Texture2D DeepLava { get; private set; }
        public Texture2D DeepWater { get; private set; }
        public Texture2D Dirt { get; private set; }
        public Texture2D Exclamation { get; private set; }
        public Texture2D ExitArea { get; private set; }
        public Texture2D ExitDoor { get; private set; }
        public Texture2D Fence { get; private set; }
        //public Texture2D Foreground { get; private set; }
        //public Texture2D ForegroundWater { get; private set; }
        public Texture2D Grass { get; private set; }
        public Texture2D GrassCliffLeft { get; private set; }
        public Texture2D GrassCliffRight { get; private set; }
        public Texture2D HeartEmpty { get; private set; }
        public Texture2D HeartFull { get; private set; }
        public Texture2D HeartHalf { get; private set; }
        public Texture2D HillLarge { get; private set; }
        public Texture2D HillSmall { get; private set; }
        public Texture2D HillLargeInWater { get; private set; }
        public Texture2D HillSmallInWater { get; private set; }
        public Texture2D Ice { get; private set; }
        public Texture2D IceCliffLeft { get; private set; }
        public Texture2D IceCliffRight { get; private set; }
        public Texture2D InvisibleTile { get; private set; }
        public Texture2D Lava { get; private set; }
        public Texture2D Rock { get; private set; }
        public Texture2D RockCliffLeft { get; private set; }
        public Texture2D RockCliffRight { get; private set; }
        public Texture2D RockDirt { get; private set; }
        public Texture2D Sand { get; private set; }
        public Texture2D SandCliffLeft { get; private set; }
        public Texture2D SandCliffRight { get; private set; }
        public Texture2D Water { get; private set; }

        public CharacterAnimation CharacterAnimation
        {
            get
            {
                List<AnimFrame> normalFrames = new List<AnimFrame> {
                    new AnimFrame(_gameCharacterImg4, 30),
                    new AnimFrame(_gameCharacterImg5, 60),
                    new AnimFrame(_gameCharacterImg4, 90),
                    new AnimFrame(_gameCharacterImg3, 120),
                    new AnimFrame(_gameCharacterImg4, 150)};

                List<AnimFrame> hitFrames = new List<AnimFrame>() {
                    new AnimFrame(_characterHitImg4, 30),
                    new AnimFrame(_characterHitImg5, 60),
                    new AnimFrame(_characterHitImg4, 90),
                    new AnimFrame(_characterHitImg3, 120),
                    new AnimFrame(_characterHitImg4, 150)};

                return new CharacterAnimation(normalFrames, hitFrames);
            }
        }

        public CharacterAnimation SwimmingCharacterAnimation
        {
            get
            {
                List<AnimFrame> normalFrames = new List<AnimFrame> {
                    new AnimFrame(_swimmingGameCharacter1, 30),
                    new AnimFrame(_swimmingGameCharacter2, 60),
                    new AnimFrame(_swimmingGameCharacter3, 90)
                };

                List<AnimFrame> hitFrames = new List<AnimFrame>() {
                    new AnimFrame(_swimmingCharacterHit1, 30),
                    new AnimFrame(_swimmingCharacterHit2, 60),
                    new AnimFrame(_swimmingCharacterHit3, 90)
                };

                return new CharacterAnimation(normalFrames, hitFrames);
            }
        }

        public EnemyAnimation RedEnemyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthRedFrames = new List<AnimFrame>() {
                    new AnimFrame(_redEnemyFullLifeImg4, 100),
                    new AnimFrame(_redEnemyFullLifeImg5, 200),
                    new AnimFrame(_redEnemyFullLifeImg4, 300),
                    new AnimFrame(_redEnemyFullLifeImg5, 400)
                };

                List<AnimFrame> halfHealthRedFrames = new List<AnimFrame>() {
                    new AnimFrame(_redEnemyHalfLifeImg4, 100),
                    new AnimFrame(_redEnemyHalfLifeImg5, 200),
                    new AnimFrame(_redEnemyHalfLifeImg4, 300),
                    new AnimFrame(_redEnemyHalfLifeImg5, 400)
                };
                List<AnimFrame> littleHealthRedFrames = new List<AnimFrame> {
                    new AnimFrame(_redEnemyLittleLifeImg4, 100),
                    new AnimFrame(_redEnemyLittleLifeImg5, 200),
                    new AnimFrame(_redEnemyLittleLifeImg4, 300),
                    new AnimFrame(_redEnemyLittleLifeImg5, 400)
                };

                return new EnemyAnimation(fullHealthRedFrames, halfHealthRedFrames, littleHealthRedFrames);
            }
        }

        public EnemyAnimation BlackEnemyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthBlackFrames = new List<AnimFrame> {
                    new AnimFrame(_blackEnemyFullLifeImg4, 10),
                    new AnimFrame(_blackEnemyFullLifeImg5, 20),
                    new AnimFrame(_blackEnemyFullLifeImg4, 30),
                    new AnimFrame(_blackEnemyFullLifeImg5, 40)
                };

                List<AnimFrame> halfHealthBlackFrames = new List<AnimFrame> {
                    new AnimFrame(_blackEnemyHalfLifeImg4, 10),
                    new AnimFrame(_blackEnemyHalfLifeImg5, 20),
                    new AnimFrame(_blackEnemyHalfLifeImg4, 30),
                    new AnimFrame(_blackEnemyHalfLifeImg5, 40)
                };

                List<AnimFrame> littleHealthBlackFrames = new List<AnimFrame> {
                    new AnimFrame(_blackEnemyLittleLifeImg4, 10),
                    new AnimFrame(_blackEnemyLittleLifeImg5, 20),
                    new AnimFrame(_blackEnemyLittleLifeImg4, 30),
                    new AnimFrame(_blackEnemyLittleLifeImg5, 40)
                };

                return new EnemyAnimation(fullHealthBlackFrames, halfHealthBlackFrames, littleHealthBlackFrames);
            }
        }

        public EnemyAnimation YellowEnemyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthYellowFrames = new List<AnimFrame> {
                    new AnimFrame(_yellowEnemyFullLifeImg4, 10),
                    new AnimFrame(_yellowEnemyFullLifeImg5, 20),
                    new AnimFrame(_yellowEnemyFullLifeImg4, 30),
                    new AnimFrame(_yellowEnemyFullLifeImg5, 40)
                };

                return new EnemyAnimation(fullHealthYellowFrames, fullHealthYellowFrames, fullHealthYellowFrames);
            }
        }

        public EnemyAnimation BlackFishAnimation
        {
            get
            {
                List<AnimFrame> fullHealthBlackFishFrames = new List<AnimFrame> {
                    new AnimFrame(_blackFishSwim1, 6),
                    new AnimFrame(_blackFishSwim2, 12)
                };

                return new EnemyAnimation(fullHealthBlackFishFrames, fullHealthBlackFishFrames, fullHealthBlackFishFrames);
            }
        }

        public EnemyAnimation GreenFishAnimation
        {
            get
            {
                List<AnimFrame> fullHealthGreenFishFrames = new List<AnimFrame> {
                    new AnimFrame(_greenFishSwim1, 6),
                    new AnimFrame(_greenFishSwim2, 12)
                };

                return new EnemyAnimation(fullHealthGreenFishFrames, fullHealthGreenFishFrames, fullHealthGreenFishFrames);
            }
        }

        public EnemyAnimation RedFishAnimation
        {
            get
            {
                List<AnimFrame> fullHealthRedFishFrames = new List<AnimFrame> {
                    new AnimFrame(_redFishSwim1, 6),
                    new AnimFrame(_redFishSwim2, 12)
                };

                return new EnemyAnimation(fullHealthRedFishFrames, fullHealthRedFishFrames, fullHealthRedFishFrames);
            }
        }

        public EnemyAnimation YellowFishAnimation
        {
            get
            {
                List<AnimFrame> fullHealthYellowFishFrames = new List<AnimFrame> {
                    new AnimFrame(_yellowFishSwim1, 6),
                    new AnimFrame(_yellowFishSwim2, 12)
                };

                return new EnemyAnimation(fullHealthYellowFishFrames, fullHealthYellowFishFrames, fullHealthYellowFishFrames);
            }
        }

        public EnemyAnimation BlackFlyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthBlackFlyFrames = new List<AnimFrame> {
                    new AnimFrame(_blackFly1, 6),
                    new AnimFrame(_blackFly2, 12)
                };

                return new EnemyAnimation(fullHealthBlackFlyFrames, fullHealthBlackFlyFrames, fullHealthBlackFlyFrames);
            }
        }

        public EnemyAnimation GreenFlyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthGreenFlyFrames = new List<AnimFrame> {
                    new AnimFrame(_greenFly1, 6),
                    new AnimFrame(_greenFly2, 12)
                };

                return new EnemyAnimation(fullHealthGreenFlyFrames, fullHealthGreenFlyFrames, fullHealthGreenFlyFrames);
            }
        }

        public EnemyAnimation RedFlyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthRedFlyFrames = new List<AnimFrame> {
                    new AnimFrame(_redFly1, 6),
                    new AnimFrame(_redFly2, 12)
                };

                return new EnemyAnimation(fullHealthRedFlyFrames, fullHealthRedFlyFrames, fullHealthRedFlyFrames);
            }
        }

        public EnemyAnimation YellowFlyAnimation
        {
            get
            {
                List<AnimFrame> fullHealthYellowFlyFrames = new List<AnimFrame> {
                    new AnimFrame(_yellowFly1, 6),
                    new AnimFrame(_yellowFly2, 12)
                };

                return new EnemyAnimation(fullHealthYellowFlyFrames, fullHealthYellowFlyFrames, fullHealthYellowFlyFrames);
            }
        }

        public InfiniteAnimation FireballAnimation
        {
            get
            {
                return new InfiniteAnimation(new List<AnimFrame> {
                    new AnimFrame(_fireball01, 5),
                    new AnimFrame(_fireball02, 10),
                    new AnimFrame(_fireball03, 15),
                    new AnimFrame(_fireball04, 20)
                });
            }
        }

        public InfiniteAnimation ProjectileAnimation
        {
            get
            {
                return new InfiniteAnimation(new List<AnimFrame> {
                    new AnimFrame(_projectile01, 10),
                    new AnimFrame(_projectile02, 20),
                    new AnimFrame(_projectile03, 30),
                    new AnimFrame(_projectile02, 40),
                    new AnimFrame(_projectile01, 50)
                });
            }
        }

        public InfiniteAnimation BronzeCoinAnimation
        {
            get
            {
                return new InfiniteAnimation(new List<AnimFrame> {
                    new AnimFrame(_coin_bronze01, 10),
                    new AnimFrame(_coin_bronze02, 20),
                    new AnimFrame(_coin_bronze03, 30),
                    new AnimFrame(_coin_bronze04, 40),
                    new AnimFrame(_coin_bronze05, 50),
                    new AnimFrame(_coin_bronze04, 60),
                    new AnimFrame(_coin_bronze03, 70),
                    new AnimFrame(_coin_bronze02, 80),
                    new AnimFrame(_coin_bronze01, 90)
                });
            }
        }

        public InfiniteAnimation SilverCoinAnimation
        {
            get
            {
                return new InfiniteAnimation(new List<AnimFrame> {
                    new AnimFrame(_coin_silver01, 10),
                    new AnimFrame(_coin_silver02, 20),
                    new AnimFrame(_coin_silver03, 30),
                    new AnimFrame(_coin_silver04, 40),
                    new AnimFrame(_coin_silver05, 50),
                    new AnimFrame(_coin_silver04, 60),
                    new AnimFrame(_coin_silver03, 70),
                    new AnimFrame(_coin_silver02, 80),
                    new AnimFrame(_coin_silver01, 90)
                });
            }
        }

        public InfiniteAnimation GoldCoinAnimation
        {
            get
            {
                return new InfiniteAnimation(new List<AnimFrame> {
                    new AnimFrame(_coin_gold01, 10),
                    new AnimFrame(_coin_gold02, 20),
                    new AnimFrame(_coin_gold03, 30),
                    new AnimFrame(_coin_gold04, 40),
                    new AnimFrame(_coin_gold05, 50),
                    new AnimFrame(_coin_gold04, 60),
                    new AnimFrame(_coin_gold03, 70),
                    new AnimFrame(_coin_gold02, 80),
                    new AnimFrame(_coin_gold01, 90)
                });
            }
        }

        public List<AnimFrame> PlusOneFrames
        {
            get
            {
                return new List<AnimFrame> {
                    new AnimFrame(_plusOne01, 10),
                    new AnimFrame(_plusOne02, 20),
                    new AnimFrame(_plusOne03, 30)
                };
            }
        }

        public List<AnimFrame> PlusTwoFrames
        {
            get
            {
                return new List<AnimFrame> {
                    new AnimFrame(_plusTwo01, 10),
                    new AnimFrame(_plusTwo02, 20),
                    new AnimFrame(_plusTwo03, 30)
                };
            }
        }

        public List<AnimFrame> PlusThreeFrames
        {
            get
            {
                return new List<AnimFrame> {
                    new AnimFrame(_plusThree01, 10),
                    new AnimFrame(_plusThree02, 20),
                    new AnimFrame(_plusThree03, 30)
                };
            }
        }

        public List<AnimFrame> MinusOneFrames
        {
            get
            {
                return new List<AnimFrame> {
                    new AnimFrame(_minusOne01, 30),
                    new AnimFrame(_minusOne02, 30),
                    new AnimFrame(_minusOne03, 30)
                };
            }
        }

        public List<AnimFrame> MinusThreeFrames
        {
            get
            {
                return new List<AnimFrame> {
                    new AnimFrame(_minusThree01, 30),
                    new AnimFrame(_minusThree02, 30),
                    new AnimFrame(_minusThree03, 30)
                };
            }
        }

        #endregion Public member

        private ImagesAndAnimations() { }

        public void LoadImages(ContentManager content)
        {
            Background = content.Load<Texture2D>("Textures\\Ghostly\\background");

            BackgroundClosestWater = content.Load<Texture2D>("Textures\\Ghostly\\closest_water");
            BackgroundCloserWater = content.Load<Texture2D>("Textures\\Ghostly\\closer_water");
            BackgroundCloseWater = content.Load<Texture2D>("Textures\\Ghostly\\close_water");

            BackgroundClosestRock = content.Load<Texture2D>("Textures\\Ghostly\\closest_rock");
            BackgroundCloserRock = content.Load<Texture2D>("Textures\\Ghostly\\closer_rock");
            BackgroundCloseRock = content.Load<Texture2D>("Textures\\Ghostly\\close_rock");
            BackgroundClosest = content.Load<Texture2D>("Textures\\Ghostly\\closest");
            BackgroundCloser = content.Load<Texture2D>("Textures\\Ghostly\\closer");
            BackgroundClose = content.Load<Texture2D>("Textures\\Ghostly\\close");
            BackgroundFar = content.Load<Texture2D>("Textures\\Ghostly\\far");
            BackgroundFurther = content.Load<Texture2D>("Textures\\Ghostly\\further");
            BackgroundFurthest = content.Load<Texture2D>("Textures\\Ghostly\\furthest");
            BackgroundWater = content.Load<Texture2D>("Textures\\Ghostly\\water_bg"); //background_water;
            //Foreground = content.Load<Texture2D>("Textures\\Ghostly\\foreground");
            //ForegroundWater = content.Load<Texture2D>("Textures\\Ghostly\\foreground_water");

            CharacterJumping = content.Load<Texture2D>("Textures\\Ghostly\\jump");

            Crate = content.Load<Texture2D>("Textures\\Ghostly\\crate");
            DeepLava = content.Load<Texture2D>("Textures\\Ghostly\\deep_lava");
            DeepWater = content.Load<Texture2D>("Textures\\Ghostly\\deep_water");
            Dirt = content.Load<Texture2D>("Textures\\Ghostly\\ground_dirt");
            Exclamation = content.Load<Texture2D>("Textures\\Ghostly\\exclamation");
            ExitArea = content.Load<Texture2D>("Textures\\Ghostly\\invisible_tile");
            ExitDoor = content.Load<Texture2D>("Textures\\Ghostly\\signExit");
            Fence = content.Load<Texture2D>("Textures\\Ghostly\\fence");
            Grass = content.Load<Texture2D>("Textures\\Ghostly\\ground_grass");
            GrassCliffLeft = content.Load<Texture2D>("Textures\\Ghostly\\grassCliffLeft");
            GrassCliffRight = content.Load<Texture2D>("Textures\\Ghostly\\grassCliffRight");
            HeartEmpty = content.Load<Texture2D>("Textures\\Ghostly\\heartEmpty");
            HeartFull = content.Load<Texture2D>("Textures\\Ghostly\\heartFull");
            HeartHalf = content.Load<Texture2D>("Textures\\Ghostly\\heartHalf");
            HillLarge = content.Load<Texture2D>("Textures\\Ghostly\\hill_large");
            HillSmall = content.Load<Texture2D>("Textures\\Ghostly\\hill_small");
            HillLargeInWater = content.Load<Texture2D>("Textures\\Ghostly\\hill_large_inwater");
            HillSmallInWater = content.Load<Texture2D>("Textures\\Ghostly\\hill_small_inwater");
            Ice = content.Load<Texture2D>("Textures\\Ghostly\\snow");
            IceCliffLeft = content.Load<Texture2D>("Textures\\Ghostly\\snowCliffLeft");
            IceCliffRight = content.Load<Texture2D>("Textures\\Ghostly\\snowCliffRight");
            InvisibleTile = content.Load<Texture2D>("Textures\\Ghostly\\invisible_tile");
            Lava = content.Load<Texture2D>("Textures\\Ghostly\\lava");
            Rock = content.Load<Texture2D>("Textures\\Ghostly\\rock");
            RockCliffLeft = content.Load<Texture2D>("Textures\\Ghostly\\rockCliffLeft");
            RockCliffRight = content.Load<Texture2D>("Textures\\Ghostly\\rockCliffRight");
            RockDirt = content.Load<Texture2D>("Textures\\Ghostly\\rockDirt");
            Sand = content.Load<Texture2D>("Textures\\Ghostly\\sand");
            SandCliffLeft = content.Load<Texture2D>("Textures\\Ghostly\\sandCliffLeft");
            SandCliffRight = content.Load<Texture2D>("Textures\\Ghostly\\sandCliffRight");
            Water = content.Load<Texture2D>("Textures\\Ghostly\\water");
            LoadBlackEnemyAnimationImages(content);

            LoadRedEnemyAnimationImages(content);

            LoadCharacterAnimation(content);
            LoadSwimmingCharacterAnimation(content);

            LoadYellowEnemyAnimationImages(content);
            LoadFishAnimationImages(content);
            LoadFlyAnimationImages(content);

            LoadFireballAnimation(content);
            LoadProjectileAnimation(content);
            LoadCoinsAnimation(content);

            LoadPlusOneAnimationImages(content);
            LoadPlusTwoAnimationImages(content);
            LoadPlusThreeAnimationImages(content);

            LoadMinusAnimationImages(content);
        }

        private void LoadCharacterAnimation(ContentManager content)
        {
            _gameCharacterImg1 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0001");
            _gameCharacterImg2 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0002");
            _gameCharacterImg3 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0003");
            _gameCharacterImg4 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0004");
            _gameCharacterImg5 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0005");
            _gameCharacterImg6 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0006");
            _gameCharacterImg7 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0007");
            _gameCharacterImg8 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0008");
            _gameCharacterImg9 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0009");
            _gameCharacterImg10 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0010");
            _gameCharacterImg11 = content.Load<Texture2D>("Textures\\Ghostly\\character\\walk0011");

            _characterHitImg3 = content.Load<Texture2D>("Textures\\Ghostly\\character\\hit0003");
            _characterHitImg4 = content.Load<Texture2D>("Textures\\Ghostly\\character\\hit0004");
            _characterHitImg5 = content.Load<Texture2D>("Textures\\Ghostly\\character\\hit0005");
        }

        private void LoadSwimmingCharacterAnimation(ContentManager content)
        {
            _swimmingGameCharacter1 = content.Load<Texture2D>("Textures\\Ghostly\\water character\\swim01");
            _swimmingGameCharacter2 = content.Load<Texture2D>("Textures\\Ghostly\\water character\\swim02");
            _swimmingGameCharacter3 = content.Load<Texture2D>("Textures\\Ghostly\\water character\\swim03");

            _swimmingCharacterHit1 = content.Load<Texture2D>("Textures\\Ghostly\\water character\\swimHit01");
            _swimmingCharacterHit2 = content.Load<Texture2D>("Textures\\Ghostly\\water character\\swimHit02");
            _swimmingCharacterHit3 = content.Load<Texture2D>("Textures\\Ghostly\\water character\\swimHit03");
        }

        private void LoadRedEnemyAnimationImages(ContentManager content)
        {
            _redEnemyFullLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\red enemy\\redEnemyFullLife0004");
            _redEnemyFullLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\red enemy\\redEnemyFullLife0005");

            _redEnemyHalfLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\red enemy\\redEnemyHalfLife0004");
            _redEnemyHalfLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\red enemy\\redEnemyHalfLife0005");

            _redEnemyLittleLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\red enemy\\redEnemyLittleLife0004");
            _redEnemyLittleLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\red enemy\\redEnemyLittleLife0005");
        }

        private void LoadBlackEnemyAnimationImages(ContentManager content)
        {
            _blackEnemyFullLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\black enemy\\blackEnemyFullLife0004");
            _blackEnemyFullLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\black enemy\\blackEnemyFullLife0005");

            _blackEnemyHalfLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\black enemy\\blackEnemyHalfLife0004");
            _blackEnemyHalfLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\black enemy\\blackEnemyHalfLife0005");

            _blackEnemyLittleLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\black enemy\\blackEnemyLittleLife0004");
            _blackEnemyLittleLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\black enemy\\blackEnemyLittleLife0005");
        }

        private void LoadYellowEnemyAnimationImages(ContentManager content)
        {
            _yellowEnemyFullLifeImg4 = content.Load<Texture2D>("Textures\\Ghostly\\yellow enemy\\yellowEnemyFullLife0004");
            _yellowEnemyFullLifeImg5 = content.Load<Texture2D>("Textures\\Ghostly\\yellow enemy\\yellowEnemyFullLife0005");
        }

        private void LoadFishAnimationImages(ContentManager content)
        {
            _blackFishSwim1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\blackFishSwim1");
            _blackFishSwim2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\blackFishSwim2");

            _greenFishSwim1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\greenFishSwim1");
            _greenFishSwim2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\greenFishSwim2");

            _redFishSwim1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\redFishSwim1");
            _redFishSwim2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\redFishSwim2");

            _yellowFishSwim1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\yellowFishSwim1");
            _yellowFishSwim2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\yellowFishSwim2");
        }

        private void LoadFlyAnimationImages(ContentManager content)
        {
            _blackFly1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\blackFly1");
            _blackFly2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\blackFly2");

            _greenFly1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\greenFly1");
            _greenFly2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\greenFly2");

            _redFly1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\redFly1");
            _redFly2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\redFly2");

            _yellowFly1 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\yellowFly1");
            _yellowFly2 = content.Load<Texture2D>("Textures\\Ghostly\\enemies\\yellowFly2");
        }

        private void LoadFireballAnimation(ContentManager content)
        {
            _fireball01 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\fireball01");
            _fireball02 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\fireball02");
            _fireball03 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\fireball03");
            _fireball04 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\fireball04");
        }

        private void LoadProjectileAnimation(ContentManager content)
        {
            _projectile01 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\projectile01");
            _projectile02 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\projectile02");
            _projectile03 = content.Load<Texture2D>("Textures\\Ghostly\\projectiles\\projectile03");
        }

        private void LoadCoinsAnimation(ContentManager content)
        {
            _coin_bronze01 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_bronze01");
            _coin_bronze02 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_bronze02");
            _coin_bronze03 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_bronze03");
            _coin_bronze04 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_bronze04");
            _coin_bronze05 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_bronze05");

            _coin_silver01 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_silver01");
            _coin_silver02 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_silver02");
            _coin_silver03 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_silver03");
            _coin_silver04 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_silver04");
            _coin_silver05 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_silver05");

            _coin_gold01 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_gold01");
            _coin_gold02 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_gold02");
            _coin_gold03 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_gold03");
            _coin_gold04 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_gold04");
            _coin_gold05 = content.Load<Texture2D>("Textures\\Ghostly\\coins\\coin_gold05");
        }

        private void LoadPlusOneAnimationImages(ContentManager content)
        {
            _plusOne01 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusOne01");
            _plusOne02 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusOne02");
            _plusOne03 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusOne03");
        }
        private void LoadPlusTwoAnimationImages(ContentManager content)
        {
            _plusTwo01 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusTwo01");
            _plusTwo02 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusTwo02");
            _plusTwo03 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusTwo03");
        }
        private void LoadPlusThreeAnimationImages(ContentManager content)
        {
            _plusThree01 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusThree01");
            _plusThree02 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusThree02");
            _plusThree03 = content.Load<Texture2D>("Textures\\Ghostly\\points\\plusThree03");
        }

        private void LoadMinusAnimationImages(ContentManager content)
        {
            _minusOne01 = content.Load<Texture2D>("Textures\\Ghostly\\minus\\minusOne01");
            _minusOne02 = content.Load<Texture2D>("Textures\\Ghostly\\minus\\minusOne02");
            _minusOne03 = content.Load<Texture2D>("Textures\\Ghostly\\minus\\minusOne03");

            _minusThree01 = content.Load<Texture2D>("Textures\\Ghostly\\minus\\minusThree01");
            _minusThree02 = content.Load<Texture2D>("Textures\\Ghostly\\minus\\minusThree02");
            _minusThree03 = content.Load<Texture2D>("Textures\\Ghostly\\minus\\minusThree03");
        }
    }
}