import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Trash2, Plus, Minus, Heart, ArrowLeft, ShoppingCart, Lock, RotateCcw } from 'lucide-react';

const CartPage = () => {
  const [cartItems, setCartItems] = useState([
    {
      id: 1,
      type: 'product',
      name: 'TEDx Classic Red T-Shirt',
      brand: 'TEDx Global',
      size: 'L',
      color: 'Red',
      price: 149000,
      originalPrice: 199000,
      quantity: 1,
      image: 'https://via.placeholder.com/100x100?text=T-Shirt',
      inStock: true,
      deliveryTime: '3-5 business days',
      savings: 50000
    },
    {
      id: 2,
      type: 'ticket',
      name: 'Innovation Conference Ticket',
      brand: 'TEDxHoChiMinhCity',
      format: 'Digital',
      price: 149000,
      originalPrice: null,
      quantity: 1,
      image: 'https://via.placeholder.com/100x100?text=Ticket',
      inStock: true,
      deliveryTime: 'Instant delivery'
    },
    {
      id: 3,
      type: 'service',
      name: 'Basic Package',
      brand: 'TON',
      price: 60000000,
      originalPrice: null,
      quantity: 1,
      image: 'https://via.placeholder.com/100x100?text=Service',
      inStock: true,
      deliveryTime: ''
    },
  ]);

  const [promoCode, setPromoCode] = useState('');

  const updateQuantity = (id, change) => {
    setCartItems(items =>
      items.map(item =>
        item.id === id
          ? { ...item, quantity: Math.max(1, item.quantity + change) }
          : item
      )
    );
  };

  const removeItem = (id) => {
    setCartItems(items => items.filter(item => item.id !== id));
  };

  const subtotal = cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  const totalSavings = cartItems.reduce((sum, item) =>
    sum + ((item.originalPrice || item.price) - item.price) * item.quantity, 0);
  const shipping = 0; // Free shipping
  const tax = Math.round(subtotal * 0.1); // 10% tax
  const total = subtotal + shipping + tax;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        {/* Header with Steps */}
        <div className="mb-8">
          <Link to="/merchandise" className="inline-flex items-center text-gray-600 hover:text-primary mb-4">
            <ArrowLeft className="w-4 h-4 mr-2" />
            Back to Cart
          </Link>

          {/* Progress Steps */}
          <div className="flex items-center justify-center gap-4 mt-6">
            <div className="flex items-center">
              <div className="w-10 h-10 rounded-full bg-primary text-white flex items-center justify-center font-semibold">
                <ShoppingCart className="w-5 h-5" />
              </div>
              <span className="ml-2 font-semibold text-primary">Cart</span>
            </div>
            <div className="w-12 h-0.5 bg-gray-300"></div>
            <div className="flex items-center">
              <div className="w-10 h-10 rounded-full bg-gray-200 text-gray-500 flex items-center justify-center">
                2
              </div>
              <span className="ml-2 text-gray-500">Checkout</span>
            </div>
            <div className="w-12 h-0.5 bg-gray-300"></div>
            <div className="flex items-center">
              <div className="w-10 h-10 rounded-full bg-gray-200 text-gray-500 flex items-center justify-center">
                ✓
              </div>
              <span className="ml-2 text-gray-500">Confirmation</span>
            </div>
          </div>
        </div>

        {cartItems.length === 0 ? (
          <div className="text-center py-16 card max-w-md mx-auto">
            <ShoppingCart className="w-16 h-16 text-gray-300 mx-auto mb-4" />
            <h2 className="text-2xl font-bold mb-2">Your cart is empty</h2>
            <p className="text-gray-600 mb-6">Add items to get started</p>
            <Link to="/merchandise" className="btn-primary inline-block">
              Continue Shopping
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Cart Items */}
            <div className="lg:col-span-2 space-y-4">
              <div className="card">
                <div className="p-6 border-b">
                  <h2 className="text-xl font-bold">Shopping Cart ({cartItems.length} items)</h2>
                  <p className="text-sm text-gray-600 mt-1">
                    Your cart will be saved for 30 minutes.
                    <Link to="/register" className="text-primary hover:underline ml-1">
                      Create an account
                    </Link> to save it permanently.
                  </p>
                </div>

                <div className="divide-y">
                  {cartItems.map((item) => (
                    <div key={item.id} className="p-6">
                      <div className="flex gap-4">
                        {/* Product Image */}
                        <img
                          src={item.image}
                          alt={item.name}
                          className="w-24 h-24 object-cover rounded-lg flex-shrink-0"
                        />

                        {/* Product Info */}
                        <div className="flex-1 min-w-0">
                          <div className="flex justify-between gap-4 mb-2">
                            <div>
                              {item.type === 'ticket' && (
                                <span className="inline-block px-2 py-0.5 bg-purple-100 text-purple-700 text-xs rounded mb-1">
                                  Ticket
                                </span>
                              )}
                              {item.type === 'service' && (
                                <span className="inline-block px-2 py-0.5 bg-blue-100 text-blue-700 text-xs rounded mb-1">
                                  Service
                                </span>
                              )}
                              <h3 className="font-semibold text-lg">{item.name}</h3>
                              <p className="text-sm text-gray-600">by {item.brand}</p>
                            </div>
                            <button
                              onClick={() => removeItem(item.id)}
                              className="text-gray-400 hover:text-red-500 h-fit"
                            >
                              <Trash2 className="w-5 h-5" />
                            </button>
                          </div>

                          {/* Item Details */}
                          <div className="flex flex-wrap gap-4 text-sm text-gray-600 mb-3">
                            {item.size && <span>Size: {item.size}</span>}
                            {item.color && (
                              <span className="flex items-center gap-1">
                                Color: <div className="w-4 h-4 rounded-full border" style={{ backgroundColor: item.color.toLowerCase() }}></div> {item.color}
                              </span>
                            )}
                            {item.format && <span>Format: {item.format}</span>}
                          </div>

                          {/* Stock Status */}
                          <div className="mb-3">
                            {item.inStock ? (
                              <span className="text-sm text-green-600 font-semibold flex items-center gap-1">
                                <span className="w-2 h-2 bg-green-600 rounded-full"></span>
                                In Stock
                              </span>
                            ) : (
                              <span className="text-sm text-red-600 font-semibold">Out of Stock</span>
                            )}
                            {item.deliveryTime && (
                              <span className="text-sm text-gray-600 ml-3">• {item.deliveryTime}</span>
                            )}
                          </div>

                          {/* Price and Quantity */}
                          <div className="flex items-center justify-between">
                            <div className="flex items-center gap-3">
                              {/* Quantity Selector */}
                              <div className="flex items-center border border-gray-300 rounded-lg">
                                <button
                                  onClick={() => updateQuantity(item.id, -1)}
                                  className="p-2 hover:bg-gray-100"
                                  disabled={item.quantity <= 1}
                                >
                                  <Minus className="w-4 h-4" />
                                </button>
                                <span className="px-4 font-semibold">{item.quantity}</span>
                                <button
                                  onClick={() => updateQuantity(item.id, 1)}
                                  className="p-2 hover:bg-gray-100"
                                >
                                  <Plus className="w-4 h-4" />
                                </button>
                              </div>

                              <button className="p-2 border border-gray-300 rounded-lg hover:bg-gray-50">
                                <Heart className="w-4 h-4" />
                              </button>
                            </div>

                            {/* Price */}
                            <div className="text-right">
                              <div className="font-bold text-lg text-primary">
                                {(item.price * item.quantity).toLocaleString()} VND
                              </div>
                              {item.originalPrice && (
                                <div className="text-sm text-gray-400 line-through">
                                  {(item.originalPrice * item.quantity).toLocaleString()} VND
                                </div>
                              )}
                              {item.savings && (
                                <div className="text-sm text-green-600">
                                  Save {(item.savings * item.quantity).toLocaleString()} VND
                                </div>
                              )}
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>

            {/* Order Summary */}
            <div className="lg:col-span-1">
              <div className="card sticky top-4">
                <div className="p-6">
                  <h2 className="text-xl font-bold mb-6 flex items-center gap-2">
                    <span className="w-8 h-8 bg-gray-100 rounded-full flex items-center justify-center">
                      🏷️
                    </span>
                    Order Summary
                  </h2>

                  {/* Promo Code */}
                  <div className="mb-6">
                    <label className="block text-sm font-semibold mb-2">Promo Code</label>
                    <div className="flex gap-2">
                      <input
                        type="text"
                        value={promoCode}
                        onChange={(e) => setPromoCode(e.target.value)}
                        placeholder="Enter code"
                        className="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      />
                      <button className="px-4 py-2 bg-gray-100 rounded-lg hover:bg-gray-200 font-semibold">
                        Apply
                      </button>
                    </div>
                  </div>

                  {/* Price Breakdown */}
                  <div className="space-y-3 mb-6">
                    <div className="flex justify-between text-gray-600">
                      <span>Subtotal:</span>
                      <span>{subtotal.toLocaleString()} VND</span>
                    </div>
                    {totalSavings > 0 && (
                      <div className="flex justify-between text-green-600">
                        <span>Item Savings:</span>
                        <span>-{totalSavings.toLocaleString()} VND</span>
                      </div>
                    )}
                    <div className="flex justify-between text-gray-600">
                      <span className="flex items-center gap-1">
                        <span className="w-4 h-4 text-gray-400">🚚</span>
                        Shipping:
                      </span>
                      <span className="text-green-600 font-semibold">FREE</span>
                    </div>
                    <div className="flex justify-between text-gray-600">
                      <span>Tax (10%):</span>
                      <span>{tax.toLocaleString()} VND</span>
                    </div>
                  </div>

                  {/* Total */}
                  <div className="pt-6 border-t">
                    <div className="flex justify-between items-center mb-6">
                      <span className="text-lg font-bold">Total:</span>
                      <span className="text-2xl font-bold text-primary">
                        {total.toLocaleString()} VND
                      </span>
                    </div>

                    <Link to="/checkout" className="btn-primary w-full text-center block mb-3">
                      <Lock className="w-4 h-4 inline mr-2" />
                      Complete Order
                    </Link>

                    <Link to="/merchandise" className="btn-secondary w-full text-center block">
                      Continue Shopping
                    </Link>
                  </div>

                  {/* Security Info */}
                  <div className="mt-6 pt-6 border-t space-y-2 text-xs text-gray-600">
                    <div className="flex items-center gap-2">
                      <Lock className="w-4 h-4" />
                      <span>Secure 256-bit SSL encryption</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <RotateCcw className="w-4 h-4" />
                      <span>Free returns within 30 days</span>
                    </div>
                    <div className="text-center mt-4">
                      <span>Gift wrapping available</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Delivery Information */}
              <div className="card mt-4 bg-blue-50 border-blue-200">
                <div className="p-4">
                  <h3 className="font-semibold mb-2">📍 Delivery Information</h3>
                  <p className="text-sm text-gray-700 mb-2">
                    <strong>TEDx Classic Red T-Shirt:</strong> 3-5 business days
                  </p>
                  <p className="text-sm text-gray-600">
                    Secure checkout with SSL encryption<br />
                    Free returns within 30 days
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default CartPage;
