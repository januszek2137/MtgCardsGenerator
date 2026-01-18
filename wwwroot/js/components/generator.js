function initializeGenerator() {
  const form = document.getElementById('generateForm');
  const btn = document.getElementById('generateBtn');
  const progressContainer = document.getElementById('progressContainer');
  const progressBar = document.getElementById('progressBar');
  const statusText = document.getElementById('statusText');

  if (!form || !btn) return;

  const stepMessages = {
    step1: [
      'Finding similar cards in database...',
      'Searching through 30,000+ cards...',
      'Analyzing mana curves...',
      'Consulting the ancient archives...',
      'Rolling for card type...',
      'Shuffling the multiverse...',
      'Querying the Blind Eternities...',
      'Asking Urza for advice...',
      'Scrying for the perfect card...',
      'Checking if it\'s already banned...',
      'Tapping five mana...',
      'Rolling a d20 for inspiration...',
      'Searching for combo pieces...',
      'Consulting the oracle text...',
      'Rifling through the graveyard...',
      'Drawing from the library of Alexandria...',
      'Invoking the heart of the cards...'
    ],
    step2: [
      'AI is generating your card...',
      'Channeling creative mana...',
      'Neural networks are thinking...',
      'Crafting card mechanics...',
      'Balancing power and toughness...',
      'Writing flavor text...',
      'Consulting the Planeswalker council...',
      'Brewing something unique...',
      'Invoking machine learning spirits...',
      'Hoping it\'s not a 0/1 for 7 mana...',
      'Making sure it passes the Bolognese test...',
      'Adding deathtouch, why not?',
      'Checking if hexproof is too much...',
      'Debating flying vs. reach...',
      'Adding "dies to removal" clause...',
      'Calculating Storm count...',
      'Praying it\'s not another Colossal Dreadmaw...',
      'Trying not to create another Oko...',
      'Making it Commander-legal... maybe...',
      'Giving it vigilance because you deserve it...',
      'Asking "what would Richard Garfield do?"...'
    ],
    step3: [
      'Creating unique artwork...',
      'Painting with digital brushes...',
      'Summoning artistic vision...',
      'Rendering magical landscapes...',
      'AI artist is at work...',
      'Capturing the card\'s essence...',
      'Mixing colors from the Æther...',
      'Illustrating your creation...',
      'Conjuring visual magic...',
      'Adding more dragons... everyone loves dragons...',
      'Making sure the art is frame-worthy...',
      'Channeling inner Terese Nielsen...',
      'Drawing a sword, probably...',
      'Painting flames, lots of flames...',
      'Sketching a mysterious hooded figure...',
      'Adding dramatic lighting effects...',
      'Making it look expensive...',
      'Ensuring proper dramatic pose...',
      'Adding unnecessary particles...'
    ],
    step4: [
      'Rendering final card...',
      'Applying card frame...',
      'Adding finishing touches...',
      'Polishing the borders...',
      'Enchanting the final result...',
      'Assembling card components...',
      'Almost ready to cast...',
      'Preparing for reveal...',
      'Making it sleeve-able...',
      'Ensuring tournament legality...',
      'Adding holographic foil effect... in your mind...',
      'Triple checking for typos...',
      'Making sure mana symbols align...',
      'Spellchecking the flavor text...',
      'One last check for infinite combos...',
      'Praying it\'s not broken...',
      'Getting ready to tap out...',
      'Preparing for the stack...'
    ]
  };

  const steps = [
    { id: 'step1', progress: 15 },
    { id: 'step2', progress: 40 },
    { id: 'step3', progress: 70 },
    { id: 'step4', progress: 90 }
  ];

  function getRandomMessage(stepId) {
    const messages = stepMessages[stepId];
    return messages[Math.floor(Math.random() * messages.length)];
  }

  function advanceStep(currentStep) {
    if (currentStep > 0) {
      const prevStepEl = document.getElementById(steps[currentStep - 1].id);
      if (prevStepEl) {
        prevStepEl.classList.remove('active');
        prevStepEl.classList.add('completed');
      }
    }

    if (currentStep < steps.length) {
      const step = steps[currentStep];
      const stepEl = document.getElementById(step.id);

      if (stepEl) {
        stepEl.classList.add('active');
      }

      if (progressBar) {
        progressBar.style.width = step.progress + '%';
      }

      if (statusText) {
        statusText.textContent = getRandomMessage(step.id);
      }

      const delay = 2000 + Math.random() * 2000;
      setTimeout(() => advanceStep(currentStep + 1), delay);
    } else {
      if (progressBar) {
        progressBar.style.width = '100%';
      }

      if (statusText) {
        const finalMessages = [
          'Almost there...',
          'Finalizing your card...',
          'Just a moment more...',
          'Preparing the grand reveal...',
          'Your card is almost ready...',
          'Resolving on the stack...',
          'No counterspells? Good!',
          'Drawing your masterpiece...',
          'And... done! No mulligans needed!',
          'Ta-da! May it bring you many victories!'
        ];
        statusText.textContent = finalMessages[Math.floor(Math.random() * finalMessages.length)];
      }
    }
  }

  form.addEventListener('submit', function(e) {
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Generating...';

    if (progressContainer) {
      progressContainer.classList.add('active');
      advanceStep(0);
    }
  });
}

document.addEventListener('DOMContentLoaded', () => {
  initializeGenerator();
});

window.initializeGenerator = initializeGenerator;
