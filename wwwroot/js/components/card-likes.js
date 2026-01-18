function initializeLikeSystem(isAuthenticated, antiForgeryToken) {
  if (!isAuthenticated) return;

  const likeButtons = document.querySelectorAll('.like-btn');

  likeButtons.forEach(button => {
    button.addEventListener('click', async function(e) {
      e.preventDefault();

      if (this.disabled) return;

      this.disabled = true;

      const cardId = this.dataset.cardId;

      try {
        const formData = new FormData();
        formData.append('__RequestVerificationToken', antiForgeryToken);

        const response = await fetch(`/Home/Like/${cardId}`, {
          method: 'POST',
          body: formData
        });

        if (response.ok) {
          const result = await response.json();

          const allButtonsForCard = document.querySelectorAll(`[data-card-id="${cardId}"].like-btn`);
          allButtonsForCard.forEach(btn => {
            if (result.isLiked) {
              btn.classList.add('liked');
              const icon = btn.querySelector('i');
              if (icon) {
                icon.classList.remove('far');
                icon.classList.add('fas');
              }
            } else {
              btn.classList.remove('liked');
              const icon = btn.querySelector('i');
              if (icon) {
                icon.classList.remove('fas');
                icon.classList.add('far');
              }
            }
          });

          const likeCountElements = document.querySelectorAll(`[data-card-id="${cardId}"] .like-count`);
          likeCountElements.forEach(el => {
            el.textContent = result.likes;
          });
        } else if (response.status === 401) {
          window.location.href = '/Identity/Account/Login';
        }
      } catch (error) {
        console.error('Error toggling like:', error);
      } finally {
        this.disabled = false;
      }
    });
  });
}

window.initializeLikeSystem = initializeLikeSystem;
