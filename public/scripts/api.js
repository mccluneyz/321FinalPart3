// API base URL
const API_BASE_URL = 'http://localhost:3000/api';

// Fetch all shops
async function fetchShops() {
  try {
    const response = await fetch(`${API_BASE_URL}/shops`);
    if (!response.ok) {
      // Try to get error message from response
      let errorMessage = `HTTP error! status: ${response.status}`;
      try {
        const errorData = await response.json();
        errorMessage = errorData.error || errorMessage;
      } catch (e) {
        // If response isn't JSON, use status message
      }
      const error = new Error(errorMessage);
      error.response = response;
      throw error;
    }
    return await response.json();
  } catch (error) {
    console.error('Error fetching shops:', error);
    throw error;
  }
}

// Add a new shop
async function addShop(shopName, rating = 0.0) {
  try {
    const response = await fetch(`${API_BASE_URL}/shops`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        ShopName: shopName,
        Rating: rating
      })
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || `HTTP error! status: ${response.status}`);
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error adding shop:', error);
    throw error;
  }
}

// Toggle favorite status
async function toggleFavorite(shopId) {
  try {
    const response = await fetch(`${API_BASE_URL}/shops/${shopId}/favorite`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
      }
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || `HTTP error! status: ${response.status}`);
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error toggling favorite:', error);
    throw error;
  }
}

// Delete a shop (soft delete)
async function deleteShop(shopId) {
  try {
    const response = await fetch(`${API_BASE_URL}/shops/${shopId}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      }
    });
    
    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;
      try {
        const errorData = await response.json();
        errorMessage = errorData.error || errorMessage;
      } catch (e) {
        // If response isn't JSON, use status message
      }
      const error = new Error(errorMessage);
      error.response = response;
      throw error;
    }
    
    return await response.json();
  } catch (error) {
    console.error('Error deleting shop:', error);
    throw error;
  }
}

