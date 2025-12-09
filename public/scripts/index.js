// DOM elements
const shopsTableBody = document.getElementById('shopsTableBody');
const loadingSpinner = document.getElementById('loadingSpinner');
const emptyState = document.getElementById('emptyState');
const addShopForm = document.getElementById('addShopForm');
const submitShopBtn = document.getElementById('submitShopBtn');
const addShopModal = new bootstrap.Modal(document.getElementById('addShopModal'));

// Format date for display
function formatDate(dateString) {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
}

// Format rating with stars
function formatRating(rating) {
  const fullStars = Math.floor(rating);
  const hasHalfStar = rating % 1 >= 0.5;
  const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
  
  let stars = '';
  for (let i = 0; i < fullStars; i++) {
    stars += '<i class="bi bi-star-fill text-warning"></i>';
  }
  if (hasHalfStar) {
    stars += '<i class="bi bi-star-half text-warning"></i>';
  }
  for (let i = 0; i < emptyStars; i++) {
    stars += '<i class="bi bi-star text-warning"></i>';
  }
  
  return `${stars} <span class="ms-2">${rating.toFixed(1)}</span>`;
}

// Render shops table
function renderShops(shops) {
  console.log('Rendering shops:', shops);
  
  if (!shops || shops.length === 0) {
    shopsTableBody.innerHTML = '';
    emptyState.classList.remove('d-none');
    return;
  }
  
  emptyState.classList.add('d-none');
  
  // Handle both camelCase and PascalCase property names
  shopsTableBody.innerHTML = shops.map(shop => {
    // Handle property name variations
    const shopName = shop.ShopName || shop.shopName || shop.name || 'Unknown';
    const rating = shop.Rating !== undefined ? shop.Rating : (shop.rating !== undefined ? shop.rating : 0);
    const dateEntered = shop.DateEntered || shop.dateEntered || shop.dateEntered || new Date().toISOString();
    const favorited = shop.Favorited !== undefined ? shop.Favorited : (shop.favorited !== undefined ? shop.favorited : false);
    const shopID = shop.ShopID !== undefined ? shop.ShopID : (shop.shopID !== undefined ? shop.shopID : shop.id);
    
    console.log('Rendering shop:', { shopName, rating, dateEntered, favorited, shopID });
    
    return `
    <tr>
      <td class="fw-semibold">${escapeHtml(shopName)}</td>
      <td>${formatRating(parseFloat(rating))}</td>
      <td>${formatDate(dateEntered)}</td>
      <td>
        <button 
          class="btn btn-sm ${favorited ? 'btn-warning' : 'btn-outline-warning'}" 
          onclick="handleFavorite(${shopID})"
          title="${favorited ? 'Unfavorite' : 'Favorite'}"
        >
          <i class="bi ${favorited ? 'bi-star-fill' : 'bi-star'}"></i>
        </button>
      </td>
      <td class="text-end">
        <button 
          class="btn btn-sm btn-danger" 
          onclick="handleDelete(${shopID}, '${escapeHtml(shopName)}')"
          title="Delete shop"
        >
          <i class="bi bi-trash"></i> Delete
        </button>
      </td>
    </tr>
  `;
  }).join('');
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
  const div = document.createElement('div');
  div.textContent = text;
  return div.innerHTML;
}

// Handle favorite toggle
async function handleFavorite(shopId) {
  try {
    await toggleFavorite(shopId);
    await loadShops();
  } catch (error) {
    alert('Error updating favorite status: ' + error.message);
  }
}

// Handle delete
async function handleDelete(shopId, shopName) {
  if (!confirm(`Are you sure you want to delete "${shopName}"?`)) {
    return;
  }
  
  try {
    await deleteShop(shopId);
    await loadShops();
  } catch (error) {
    alert('Error deleting shop: ' + error.message);
  }
}

// Load shops from API
async function loadShops() {
  try {
    loadingSpinner.classList.remove('d-none');
    shopsTableBody.innerHTML = '';
    
    const shops = await fetchShops();
    console.log('Shops received from API:', shops);
    console.log('Number of shops:', shops.length);
    if (shops.length > 0) {
      console.log('First shop data:', shops[0]);
      console.log('First shop properties:', Object.keys(shops[0]));
    }
    renderShops(shops);
  } catch (error) {
    console.error('Error loading shops:', error);
    let errorMessage = error.message;
    
    // Try to get more detailed error from response
    if (error.response) {
      try {
        const errorData = await error.response.json();
        errorMessage = errorData.error || error.message;
      } catch (e) {
        // If we can't parse the error, use the original message
      }
    }
    
    shopsTableBody.innerHTML = `
      <tr>
        <td colspan="5" class="text-center text-danger">
          <i class="bi bi-exclamation-triangle me-2"></i>
          Error loading shops: ${errorMessage}
          <br><small>Make sure the server is running and the database is connected.</small>
        </td>
      </tr>
    `;
  } finally {
    loadingSpinner.classList.add('d-none');
  }
}

// Handle form submission
submitShopBtn.addEventListener('click', async (e) => {
  e.preventDefault();
  
  const shopName = document.getElementById('shopName').value.trim();
  const shopRating = parseFloat(document.getElementById('shopRating').value) || 0.0;
  
  if (!shopName) {
    alert('Please enter a shop name');
    return;
  }
  
  if (shopRating < 0 || shopRating > 5) {
    alert('Rating must be between 0.0 and 5.0');
    return;
  }
  
  try {
    submitShopBtn.disabled = true;
    submitShopBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Adding...';
    
    const result = await addShop(shopName, shopRating);
    console.log('Shop added:', result);
    
    // Reset form and close modal
    addShopForm.reset();
    addShopModal.hide();
    
    // Small delay to ensure database has updated
    await new Promise(resolve => setTimeout(resolve, 500));
    
    // Reload shops
    await loadShops();
    
    // Show success message
    alert('Shop added successfully!');
  } catch (error) {
    console.error('Error adding shop:', error);
    alert('Error adding shop: ' + error.message);
  } finally {
    submitShopBtn.disabled = false;
    submitShopBtn.innerHTML = '<i class="bi bi-plus-circle me-2"></i>Add Shop';
  }
});

// Make functions globally available
window.handleFavorite = handleFavorite;
window.handleDelete = handleDelete;

// Load shops on page load
document.addEventListener('DOMContentLoaded', () => {
  loadShops();
});

