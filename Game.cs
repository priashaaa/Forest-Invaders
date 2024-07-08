namespace COIS2020.priashabarua0778496.Assignment3
{
    using Microsoft.Xna.Framework; // Needed for Vector2
    using TrentCOIS.Tools.Visualization;
    using COIS2020.StarterCode.Assignment3;

    using System;

    public class CastleDefender : Visualization
    {
        public LinkedList<Wizard> WizardSquad { get; private set; }
        public Queue<Wizard> RecoveryQueue { get; private set; }

        public LinkedList<Goblin> GoblinSquad { get; private set; }
        public LinkedList<Goblin> BackupGoblins { get; private set; }

        public LinkedList<Spell> Spells { get; private set; }
        public Node<Wizard>? ActiveWizard { get; private set; }

        private uint nextSpellTime;

        private Vector2 goblinDirection;
        
        // Courtyard boundaries
        private static readonly Vector2 FieldTL = CastleGameRenderer.FieldTL;
        private static readonly Vector2 FieldBR = CastleGameRenderer.FieldBR;

        private Random random;


        public CastleDefender()
        {
            WizardSquad = new LinkedList<Wizard>();
            RecoveryQueue = new Queue<Wizard>();

            GoblinSquad = new LinkedList<Goblin>();
            BackupGoblins = new LinkedList<Goblin>();

            Spells = new LinkedList<Spell>();

            random = new Random();

            InitializeGame();
        }

        private void InitializeGame()
        {
            // Create eight wizards and add them to the main WizardSquad
            for (int i = 0; i < 8; i++)
            {
                WizardSquad.AddBack(new Wizard());
            }

            // Create eight goblins and add them to the main GoblinSquad
            for (int i = 0; i < 8; i++)
            {
                GoblinSquad.AddBack(new Goblin());
            }

            // Create six backup goblins and add them to the BackupGoblins list
            for (int i = 0; i < 6; i++)
            {
                BackupGoblins.AddBack(new Goblin());
            }

            // Pick a random direction for the goblin squad to walk in
            goblinDirection = new Vector2((float)random.NextDouble(), (float)random.NextDouble());
            goblinDirection.Normalize(); // Ensure the vector has a length of 1.0

            // Mark the first wizard as active by getting its node from the linked list
            ActiveWizard = WizardSquad.Head;

            // Set the nextSpellTime so the first wizard doesn't fire immediately after spawning
            nextSpellTime = (uint)(15 + random.Next(-5, 6)); // Random number between 10 and 20
        }

        private void UpdateSpell(uint currentFrame)
        {
            Node<Spell>? spellNode = Spells.Head;
            while (spellNode != null)
            {
                Node<Spell>? nextSpellNode = spellNode.Next;

                // Move the spell up the screen (negative Y-direction) with a speed of 0.75f
                spellNode.Item.Move(new Vector2(0, -1), Spell.Speed);

                // Check if the spell is off the screen
                if (CastleGameRenderer.IsOffScreen(spellNode.Item.Position))
                {       
            
                    Spells.Remove(spellNode);
                    
                }

                spellNode = nextSpellNode;
            }
          
        }

        private void UpdateGoblin(uint currentFrame)
        {
            Node<Goblin>? goblinHead = GoblinSquad.Head;
            if (goblinHead == null) return;

            // Move the head goblin
            Vector2 prevPos = goblinHead.Item.Position;
            goblinHead.Item.Move(goblinDirection, Goblin.Speed);

            // Check for boundary collisions and adjust direction using CheckWallCollision
            CastleGameRenderer.CheckWallCollision(goblinHead.Item, ref goblinDirection);

            // Move the rest of the goblins
            Node<Goblin>? curr = goblinHead.Next;
            while (curr != null)
            {
                Vector2 currentPos = curr.Item.Position;
                curr.Item.Position = prevPos;
                prevPos = currentPos;
                curr = curr.Next;
            }

            // Check for collisions
            curr = GoblinSquad.Head;
            while (curr != null)
            {
                bool goblinHit = false;
                Node<Spell>? spellNode = Spells.Head;
                while (spellNode != null)
                {
                    if (CombatEntity.Colliding(curr.Item, spellNode.Item))
                    {
                        // Remove the goblin and the spell
                        Node<Goblin>? goblinToRemove = curr;
                        curr = curr.Next;
                        GoblinSquad.Remove(goblinToRemove);
                        Spells.Remove(spellNode);

                        // Change the goblin direction
                        goblinDirection = new Vector2((float)random.NextDouble(), (float)random.NextDouble());
                        goblinDirection.Normalize();

                        goblinHit = true;
                        break;
                    }
                    spellNode = spellNode.Next;
                }
                if (!goblinHit)
                {
                    curr = curr.Next;
                }
            }

            // Check if goblins need reinforcements
            int initialGoblinCount = 8; // Initial number of goblins
            if (GoblinSquad.Count() <= initialGoblinCount / 2 && !BackupGoblins.IsEmpty)
            {
                GoblinSquad.AppendAll(BackupGoblins);
            }

            // Check if all goblins have been defeated
            CheckForDefeatedGoblins();
        }

        private void CheckForDefeatedGoblins()
        {
            if (GoblinSquad.IsEmpty)
            {
                Console.WriteLine("All goblins have been defeated!");
                Pause(); // Call the Pause method to stop the simulation
            }
        }

        private void UpdateWizard(uint currentFrame)
        {
            // Get the spell type, position, energy, and spell level of the active wizard
            Element spellType = ActiveWizard.Item.SpellType;
            Vector2 wizardPos = ActiveWizard.Item.Position;

            // Check if it is time for the active wizard to cast a spell
            if (nextSpellTime <= 0)
            {
                // If the wizard has energy, cast the spell and reduce their energy
                if (ActiveWizard.Item.Energy - ActiveWizard.Item.SpellLevel >= 0)
                {
                    Spell castedSpell = new Spell(spellType, wizardPos);
                    Spells.AddBack(castedSpell);
                    ActiveWizard.Item.Energy -= ActiveWizard.Item.SpellLevel;
                }
                // else
                // {
                //     // If the wizard is out of energy, move them to the recovery queue
                //     RecoveryQueue.Enqueue(ActiveWizard.Item);
                //     WizardSquad.Remove(ActiveWizard.Item);
                // }

                // Reset the next spell time to a random value between 10 and 20 frames
                nextSpellTime = (uint)(15 + random.Next(-5, 6));

                // Move to the next wizard in the chain or wrap around to the head of the list
                if (ActiveWizard.Next != null)
                {
                    ActiveWizard = ActiveWizard.Next;
                }
                else
                {
                    ActiveWizard = WizardSquad.Head;
                }
            }

            foreach(var wizard in WizardSquad){

                if(wizard.Energy - wizard.SpellLevel < 0){

                    RecoveryQueue.Enqueue(wizard);
                    WizardSquad.Remove(wizard);

                }
            }

            // Process the recovery queue if it's not empty
            if (!RecoveryQueue.IsEmpty)
            {
                // Get the first wizard in the recovery queue
                Wizard currWizard = RecoveryQueue.Peek();

                // Replenish their energy every 5 frames
                if (currWizard.Energy < currWizard.MaxEnergy)
                {
                    if (currentFrame % 5 == 0)
                    {
                        currWizard.Energy += 1;
                    }
                    
                }
                else
                {
                    // Once the wizard is fully energized, move them to the frontlines before the active wizard
                    RecoveryQueue.Dequeue();

                    if (ActiveWizard != null)
                    {
                        WizardSquad.InsertBefore(ActiveWizard, currWizard);
                    }
                    else
                    {
                        WizardSquad.AddFront(currWizard);
                    }
                }
            }
        }

        protected override void Update(uint currentFrame)
        {
            UpdateSpell(currentFrame);
            UpdateGoblin(currentFrame);
            UpdateWizard(currentFrame);
            nextSpellTime -= 1;
        }
    }
}
