import { createContext, useState, useEffect, useContext } from 'react';
import cartService from '../services/cartService';
import { AuthContext } from './AuthContext';

export const CartContext = createContext();

export const CartProvider = ({ children }) => {
  const [cartItems, setCartItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const { isAuthenticated } = useContext(AuthContext);

  // Load cart from localStorage or API
  useEffect(() => {
    if (isAuthenticated) {
      loadCartFromAPI();
    } else {
      loadCartFromLocalStorage();
    }
  }, [isAuthenticated]);

  const loadCartFromAPI = async () => {
    try {
      setLoading(true);
      const data = await cartService.getCart();
      setCartItems(data.items || []);
    } catch (error) {
      console.error('Failed to load cart:', error);
      loadCartFromLocalStorage();
    } finally {
      setLoading(false);
    }
  };

  const loadCartFromLocalStorage = () => {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      setCartItems(JSON.parse(savedCart));
    }
  };

  const saveCartToLocalStorage = (items) => {
    localStorage.setItem('cart', JSON.stringify(items));
  };

  const addToCart = async (item) => {
    try {
      if (isAuthenticated) {
        await cartService.addToCart(item);
        await loadCartFromAPI();
      } else {
        const existingItemIndex = cartItems.findIndex(
          cartItem => cartItem.id === item.id && cartItem.type === item.type
        );

        let updatedCart;
        if (existingItemIndex > -1) {
          updatedCart = [...cartItems];
          updatedCart[existingItemIndex].quantity += item.quantity || 1;
        } else {
          updatedCart = [...cartItems, { ...item, quantity: item.quantity || 1 }];
        }

        setCartItems(updatedCart);
        saveCartToLocalStorage(updatedCart);
      }
      return { success: true };
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to add to cart' };
    }
  };

  const updateQuantity = async (itemId, quantity) => {
    try {
      if (isAuthenticated) {
        await cartService.updateCartItem(itemId, quantity);
        await loadCartFromAPI();
      } else {
        const updatedCart = cartItems.map(item =>
          item.id === itemId ? { ...item, quantity: Math.max(1, quantity) } : item
        );
        setCartItems(updatedCart);
        saveCartToLocalStorage(updatedCart);
      }
      return { success: true };
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to update quantity' };
    }
  };

  const removeFromCart = async (itemId) => {
    try {
      if (isAuthenticated) {
        await cartService.removeFromCart(itemId);
        await loadCartFromAPI();
      } else {
        const updatedCart = cartItems.filter(item => item.id !== itemId);
        setCartItems(updatedCart);
        saveCartToLocalStorage(updatedCart);
      }
      return { success: true };
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to remove item' };
    }
  };

  const clearCart = async () => {
    try {
      if (isAuthenticated) {
        await cartService.clearCart();
      } else {
        localStorage.removeItem('cart');
      }
      setCartItems([]);
      return { success: true };
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Failed to clear cart' };
    }
  };

  const getCartTotal = () => {
    return cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  };

  const getCartCount = () => {
    return cartItems.reduce((sum, item) => sum + item.quantity, 0);
  };

  const value = {
    cartItems,
    loading,
    addToCart,
    updateQuantity,
    removeFromCart,
    clearCart,
    getCartTotal,
    getCartCount,
  };

  return (
    <CartContext.Provider value={value}>
      {children}
    </CartContext.Provider>
  );
};
