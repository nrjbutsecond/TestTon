import { useState } from 'react';
import { Search, Filter, Heart, ShoppingCart, Star, Grid, List } from 'lucide-react';
import { Link } from 'react-router-dom';

const MerchandisePage = () => {
  const [viewMode, setViewMode] = useState('grid'); // 'grid' or 'list'
  const [selectedCategory, setSelectedCategory] = useState('All Products');

  const categories = ['All Products', 'Featured', 'New Arrivals', 'On Sale'];

  const products = [
    {
      id: 1,
      name: 'TON Official T-Shirt',
      brand: 'TEDx Global',
      price: 149000,
      originalPrice: 199000,
      discount: 25,
      rating: 4.8,
      reviews: 234,
      sales: 1500,
      image: 'https://via.placeholder.com/300x300?text=T-Shirt',
      status: 'Featured',
      inStock: true,
      colors: ['Red', 'Black', 'White'],
      sizes: ['S', 'M', 'L', 'XL'],
      tags: ['Comfort', 'Cotton']
    },
    {
      id: 2,
      name: 'Hue Local Morning Event',
      brand: 'TEDxHue City',
      price: 89000,
      originalPrice: null,
      discount: 0,
      rating: 4.7,
      reviews: 158,
      sales: 2340,
      image: 'https://via.placeholder.com/300x300?text=Headphones',
      status: 'New Arrivals',
      inStock: true,
      colors: ['Black', 'Red'],
      sizes: [],
      tags: ['Wireless', 'Premium']
    },
    {
      id: 3,
      name: 'TEDx The Vinh Kraft',
      brand: 'TEDxLocal/Vtown',
      price: 209000,
      originalPrice: null,
      discount: 0,
      rating: 4.9,
      reviews: 89,
      sales: 454,
      image: 'https://via.placeholder.com/300x300?text=Bag',
      status: 'On Sale',
      inStock: true,
      colors: ['Brown'],
      sizes: [],
      tags: ['Leather', 'Planning']
    },
    {
      id: 4,
      name: 'Lan Ink Ideas Collection',
      brand: 'TEDx Left',
      price: 76000,
      originalPrice: null,
      discount: 0,
      rating: 4.6,
      reviews: 87,
      sales: 435,
      image: 'https://via.placeholder.com/300x300?text=Collection',
      status: 'Limited Edition',
      inStock: false,
      colors: ['Black'],
      sizes: [],
      tags: ['Collectible']
    },
    {
      id: 5,
      name: 'Hot Coffee',
      brand: 'TEDxLine Cafe',
      price: 28000,
      originalPrice: null,
      discount: 0,
      rating: 4.6,
      reviews: 137,
      sales: 895,
      image: 'https://via.placeholder.com/300x300?text=Coffee',
      status: 'New',
      inStock: true,
      colors: [],
      sizes: [],
      tags: ['Ceramic', 'Inspirational']
    },
    {
      id: 6,
      name: 'TEDx Sustainable Tote Bag',
      brand: 'TEDxEco',
      price: 350000,
      originalPrice: null,
      discount: 0,
      rating: 4.5,
      reviews: 313,
      sales: 1120,
      image: 'https://via.placeholder.com/300x300?text=Tote+Bag',
      status: 'Best Seller',
      inStock: true,
      colors: ['Cool', 'Blue'],
      sizes: [],
      tags: ['Eco-friendly', 'Campus']
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b">
        <div className="container mx-auto px-4 py-6">
          <h1 className="text-3xl font-bold mb-6">Merchandise</h1>

          {/* Search and View Toggle */}
          <div className="flex flex-wrap gap-4 items-center justify-between">
            <div className="flex-1 min-w-[300px] relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                type="text"
                placeholder="Search products, organizations, tags..."
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div className="flex gap-2">
              <select className="px-4 py-2 border border-gray-300 rounded-lg">
                <option>Most Popular</option>
                <option>Price: Low to High</option>
                <option>Price: High to Low</option>
                <option>Newest</option>
              </select>

              <button className="p-2 border border-gray-300 rounded-lg hover:bg-gray-50">
                <Filter className="w-5 h-5" />
              </button>

              <div className="flex border border-gray-300 rounded-lg overflow-hidden">
                <button
                  onClick={() => setViewMode('grid')}
                  className={`p-2 ${viewMode === 'grid' ? 'bg-primary text-white' : 'bg-white hover:bg-gray-50'}`}
                >
                  <Grid className="w-5 h-5" />
                </button>
                <button
                  onClick={() => setViewMode('list')}
                  className={`p-2 ${viewMode === 'list' ? 'bg-primary text-white' : 'bg-white hover:bg-gray-50'}`}
                >
                  <List className="w-5 h-5" />
                </button>
              </div>

              <button className="p-2 border border-gray-300 rounded-lg hover:bg-gray-50">
                <Heart className="w-5 h-5" />
              </button>

              <Link to="/cart" className="p-2 bg-primary text-white rounded-lg hover:bg-primary-dark">
                <ShoppingCart className="w-5 h-5" />
              </Link>
            </div>
          </div>
        </div>
      </div>

      {/* Category Tabs */}
      <div className="bg-white border-b">
        <div className="container mx-auto px-4">
          <div className="flex gap-6 overflow-x-auto">
            {categories.map((category) => (
              <button
                key={category}
                onClick={() => setSelectedCategory(category)}
                className={`py-4 px-2 border-b-2 whitespace-nowrap transition-colors ${
                  selectedCategory === category
                    ? 'border-primary text-primary font-semibold'
                    : 'border-transparent text-gray-600 hover:text-gray-900'
                }`}
              >
                {category}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Products Grid */}
      <div className="container mx-auto px-4 py-8">
        <div className="mb-4 text-gray-600">
          Showing 6 of 6 products
        </div>

        <div className={viewMode === 'grid'
          ? 'grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6'
          : 'space-y-4'
        }>
          {products.map((product) => (
            <div key={product.id} className="card hover:shadow-xl transition-shadow relative group">
              {/* Product Image */}
              <div className="relative">
                {product.discount > 0 && (
                  <span className="absolute top-4 left-4 bg-red-500 text-white px-2 py-1 rounded text-xs font-semibold">
                    -{product.discount}%
                  </span>
                )}
                {product.status === 'New' && (
                  <span className="absolute top-4 left-4 bg-blue-500 text-white px-2 py-1 rounded text-xs font-semibold">
                    New
                  </span>
                )}
                {!product.inStock && (
                  <div className="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center">
                    <span className="bg-gray-500 text-white px-4 py-2 rounded-lg font-semibold">
                      Out of Stock
                    </span>
                  </div>
                )}
                <img
                  src={product.image}
                  alt={product.name}
                  className="w-full aspect-square object-cover"
                />

                {/* Quick Actions */}
                <div className="absolute top-4 right-4 flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button className="p-2 bg-white rounded-full shadow-md hover:bg-gray-100">
                    <Heart className="w-4 h-4" />
                  </button>
                </div>
              </div>

              {/* Product Info */}
              <div className="p-4">
                <div className="text-xs text-gray-500 mb-1">{product.brand}</div>
                <h3 className="font-semibold mb-2 line-clamp-2">{product.name}</h3>

                {/* Rating */}
                <div className="flex items-center gap-2 mb-2">
                  <div className="flex text-yellow-400">
                    {[...Array(5)].map((_, i) => (
                      <Star
                        key={i}
                        className={`w-4 h-4 ${i < Math.floor(product.rating) ? 'fill-current' : ''}`}
                      />
                    ))}
                  </div>
                  <span className="text-sm text-gray-600">{product.rating}</span>
                  <span className="text-sm text-gray-400">({product.reviews})</span>
                </div>

                {/* Colors */}
                {product.colors.length > 0 && (
                  <div className="flex gap-1 mb-3">
                    {product.colors.map((color, index) => (
                      <div
                        key={index}
                        className="w-6 h-6 rounded-full border-2 border-gray-300"
                        style={{ backgroundColor: color.toLowerCase() }}
                        title={color}
                      />
                    ))}
                  </div>
                )}

                {/* Price */}
                <div className="flex items-center gap-2 mb-3">
                  <span className="text-primary font-bold">
                    {product.price.toLocaleString()} VND
                  </span>
                  {product.originalPrice && (
                    <span className="text-sm text-gray-400 line-through">
                      {product.originalPrice.toLocaleString()} VND
                    </span>
                  )}
                </div>

                {/* Delivery Info */}
                <div className="text-xs text-gray-600 mb-3">
                  {product.inStock ? (
                    <>
                      <span className="text-green-600 font-semibold">In Stock</span>
                      <span className="ml-2">Delivery in 3-5 days</span>
                    </>
                  ) : (
                    <span className="text-red-600 font-semibold">Out of Stock</span>
                  )}
                </div>

                {/* Tags */}
                {product.tags.length > 0 && (
                  <div className="flex flex-wrap gap-1 mb-3">
                    {product.tags.map((tag, index) => (
                      <span key={index} className="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded">
                        {tag}
                      </span>
                    ))}
                  </div>
                )}

                {/* Add to Cart Button */}
                <button
                  disabled={!product.inStock}
                  className={`w-full py-2 rounded-lg font-semibold transition-colors ${
                    product.inStock
                      ? 'bg-primary text-white hover:bg-primary-dark'
                      : 'bg-gray-300 text-gray-600 cursor-not-allowed'
                  }`}
                >
                  <ShoppingCart className="w-4 h-4 inline mr-2" />
                  {product.inStock ? 'Add to Cart' : 'Out of Stock'}
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default MerchandisePage;
