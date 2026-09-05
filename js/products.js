/**
 * Notch Perfumes - Product Catalog Data (Dynamic DB Sync & Product Master Storage)
 */
const DEFAULT_PRODUCTS = [
    {
        id: 'notch-velvet-oud',
        code: 'PRD-1001',
        name: 'Notch Velvet Oud EDP',
        subtitle: 'For Men & Unisex • Opulent Smoky Oud',
        category: 'men',
        collection: 'night',
        scentFamily: 'amber',
        price: 295,
        originalPrice: 350,
        rating: 5.0,
        reviewsCount: 24,
        badge: 'BESTSELLER',
        image: 'https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Agarwood (Oud), Saffron, Bergamot',
        heartNotes: 'Damask Rose, Amberwood',
        baseNotes: 'Smoky Leather, Vanilla, Musk',
        perfumer: 'Master Qatari Perfumers',
        description: 'Opulent blend of aged Cambodian Oud, roasted spices, and velvety Amber.',
        sizes: ["100 ml", "50 ml"],
        stock: 150
    },
    {
        id: 'notch-steele-intense',
        code: 'PRD-1002',
        name: 'Notch Steele Intense EDP',
        subtitle: 'For Men • Bold Woods & Spices',
        category: 'men',
        collection: 'classics',
        scentFamily: 'woody',
        price: 320,
        originalPrice: 380,
        rating: 4.9,
        reviewsCount: 18,
        badge: 'NEW ARRIVAL',
        image: 'https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Italian Bergamot, Cardamom, Black Pepper',
        heartNotes: 'Vetiver, Cedarwood',
        baseNotes: 'Dark Amber, Guaiac Wood',
        perfumer: 'Notch Paris Lab',
        description: 'Magnetic masculine fragrance with crisp bergamot and grounding cedarwood.',
        sizes: ["100 ml", "50 ml"],
        stock: 90
    },
    {
        id: 'notch-royal-rose',
        code: 'PRD-1003',
        name: 'Notch Royal Rose EDP',
        subtitle: 'For Women • Sensual Damask Floral',
        category: 'women',
        collection: 'qatari-rose',
        scentFamily: 'floral',
        price: 245,
        originalPrice: 295,
        rating: 5.0,
        reviewsCount: 31,
        badge: 'POPULAR',
        image: 'https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Qatari Rose Petals, Peony',
        heartNotes: 'Jasmine Sambac, White Lily',
        baseNotes: 'Cashmere Wood, Amber',
        perfumer: 'Notch Qatar Perfumers',
        description: 'Elegant bouquet of velvet rose and fresh Qatari jasmine.',
        sizes: ["100 ml", "50 ml"],
        stock: 120
    },
    {
        id: 'notch-celeste',
        code: 'PRD-1004',
        name: 'Notch Celeste EDP',
        subtitle: 'For Women & Unisex • Aquatic Breeze',
        category: 'women',
        collection: 'mediterranean',
        scentFamily: 'oceanic',
        price: 275,
        originalPrice: 330,
        rating: 4.8,
        reviewsCount: 15,
        badge: 'FRESH',
        image: 'https://images.unsplash.com/photo-1592945403244-b3fbafd7f539?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Sea Mist, Bergamot, Lemon',
        heartNotes: 'Oceanic Notes, Water Lily',
        baseNotes: 'White Amber, Driftwood',
        perfumer: 'Notch Riviera Atelier',
        description: 'Exhilarating sea mist and luminous Mediterranean citrus notes.',
        sizes: ["100 ml", "50 ml"],
        stock: 110
    },
    {
        id: 'notch-raw-men',
        code: 'PRD-1005',
        name: 'Notch Raw Eau De Parfum',
        subtitle: 'For Men • Crisp Citrus & Patchouli',
        category: 'men',
        collection: 'classics',
        scentFamily: 'citrus',
        price: 249,
        originalPrice: 299,
        rating: 4.8,
        reviewsCount: 420,
        badge: 'Bestseller',
        image: 'https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Bergamot, Waterfruit, Mandarin',
        heartNotes: 'Violet Leaves, Pomarose, Carnation',
        baseNotes: 'Indonesian Patchouli, Guaiac Wood, Cashmeran',
        perfumer: 'Olivier Pescheux (Qatar)',
        description: 'A bold citrus explosion meeting deep grounding patchouli.',
        sizes: ["100 ml", "50 ml"],
        stock: 120
    },
    {
        id: 'notch-amalfi-bleue',
        code: 'PRD-1006',
        name: 'Notch Amalfi Bleue EDP',
        subtitle: 'Unisex • Mediterranean Aquatic Fresh',
        category: 'unisex',
        collection: 'mediterranean',
        scentFamily: 'oceanic',
        price: 139,
        originalPrice: 169,
        rating: 4.9,
        reviewsCount: 380,
        badge: 'Trending',
        image: 'https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Citrus Zest, Apple, Sea Breeze Accord',
        heartNotes: 'Clary Sage, Violet Leaf, Fig Tree',
        baseNotes: 'Ambergris, Driftwood, Vetiver',
        perfumer: 'Jordi Fernandez (Qatar)',
        description: 'Transport yourself to the sun-drenched cliffs of the Italian coastline.',
        sizes: ["90 ml", "20 ml Travel"],
        stock: 95
    },
    {
        id: 'notch-nox-him',
        code: 'PRD-1007',
        name: 'Notch Nox Eau De Parfum',
        subtitle: 'For Men • Dark Amber & Vanilla Spices',
        category: 'men',
        collection: 'night',
        scentFamily: 'amber',
        price: 169,
        originalPrice: 189,
        rating: 4.9,
        reviewsCount: 184,
        badge: 'Luxury Line',
        image: 'https://images.unsplash.com/photo-1615397349754-cfa2066a298e?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Black Pepper, Roasted Coffee, Saffron',
        heartNotes: 'Cinnamon Bark, Incense, Dark Rose',
        baseNotes: 'Rich Amber, Tonka Bean, Oud Accord',
        perfumer: 'Nathalie Lorson (Qatar)',
        description: 'Blends dark spice, rich coffee notes, and velvety amber.',
        sizes: ["100 ml"],
        stock: 85
    },
    {
        id: 'notch-noura-her',
        code: 'PRD-1008',
        name: 'Notch Noura Eau De Parfum',
        subtitle: 'For Women • Opulent Rose & Soft Musk',
        category: 'women',
        collection: 'qatari-rose',
        scentFamily: 'floral',
        price: 169,
        originalPrice: 189,
        rating: 4.9,
        reviewsCount: 210,
        badge: 'Luxury Line',
        image: 'https://images.unsplash.com/photo-1588405748880-12d1d2a59f75?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Mandarin, Red Berries, Almond Blossom',
        heartNotes: 'Damask Rose, Iris, Magnolia',
        baseNotes: 'Cashmere Wood, Vanilla Pod, Crystal Musk',
        perfumer: 'Michel Girard (Qatar)',
        description: 'Timeless elegance. A sensual rose bloom wrapped in golden vanilla.',
        sizes: ["100 ml"],
        stock: 90
    },
    {
        id: 'notch-discovery-kit',
        code: 'PRD-1009',
        name: 'Notch Discovery Gift Box',
        subtitle: 'Unisex • 5 x 5ml Fragrance Miniatures',
        category: 'discovery',
        collection: 'classics',
        scentFamily: 'citrus',
        price: 45,
        originalPrice: 59,
        rating: 4.9,
        reviewsCount: 890,
        badge: 'Must Try',
        image: 'https://images.unsplash.com/photo-1547887537-6158d64c35b3?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Assorted Top Notes (Citrus, Marine, Floral, Spice)',
        heartNotes: 'Curated Qatari Essences',
        baseNotes: 'Long-lasting Amber & Wood bases',
        perfumer: 'Master Qatari Perfumers',
        description: 'Explore Notch top icons in travel-ready miniature bottles.',
        sizes: ["5 x 5ml Set"],
        stock: 210
    },
    {
        id: 'notch-nude-women',
        code: 'PRD-1010',
        name: 'Notch Nude Eau De Parfum',
        subtitle: 'For Women • Romantic Lychee & Powdery Rose',
        category: 'women',
        collection: 'qatari-rose',
        scentFamily: 'floral',
        price: 119,
        originalPrice: 139,
        rating: 4.7,
        reviewsCount: 310,
        badge: 'Fan Favorite',
        image: 'https://images.unsplash.com/photo-1592945403244-b3fbafd7f539?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Lychee, Bergamot, Raspberry',
        heartNotes: 'Rose Petals, Violet, Peony',
        baseNotes: 'Sandalwood, Vanilla, White Musk',
        perfumer: 'Alberto Morillas (Qatar)',
        description: 'Notch Nude captures pure romance. Soft fruity nuances lead into roses.',
        sizes: ["50 ml", "100 ml"],
        stock: 60
    },
    {
        id: 'notch-amber-royale',
        code: 'PRD-1011',
        name: 'Notch Amber Royale EDP',
        subtitle: 'For Men • Opulent Amber & Roasted Saffron',
        category: 'men',
        collection: 'night',
        scentFamily: 'amber',
        price: 345,
        originalPrice: 410,
        rating: 5.0,
        reviewsCount: 42,
        badge: 'EXCLUSIVE',
        image: 'https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Saffron, Cardamom, Italian Bergamot',
        heartNotes: 'Amberwood, Qatari Rose, Cinnamon',
        baseNotes: 'Rich Amber, Sandalwood, Tonka Bean',
        perfumer: 'Notch Qatar Atelier',
        description: 'Opulent golden amber infused with rare saffron and royal spices.',
        sizes: ["100 ml", "50 ml"],
        stock: 80
    },
    {
        id: 'notch-pearl-jasmine',
        code: 'PRD-1012',
        name: 'Notch Pearl Jasmine EDP',
        subtitle: 'For Women • Sensual White Jasmine & Rose',
        category: 'women',
        collection: 'qatari-rose',
        scentFamily: 'floral',
        price: 285,
        originalPrice: 340,
        rating: 4.9,
        reviewsCount: 29,
        badge: 'POPULAR',
        image: 'https://images.unsplash.com/photo-1588405748880-12d1d2a59f75?auto=format&fit=crop&w=800&q=80',
        topNotes: 'White Jasmine, Orange Blossom, Pear',
        heartNotes: 'Damask Rose, Lily of the Valley',
        baseNotes: 'White Amber, Cashmere Musk, Vanilla',
        perfumer: 'Notch Paris Lab',
        description: 'Enchanting bouquet of blooming white jasmine and velvet rose.',
        sizes: ["100 ml", "50 ml"],
        stock: 110
    },
    {
        id: 'notch-aqua-horizon',
        code: 'PRD-1013',
        name: 'Notch Aqua Horizon EDP',
        subtitle: 'Unisex • Oceanic Marine Breeze & Citrus',
        category: 'unisex',
        collection: 'mediterranean',
        scentFamily: 'oceanic',
        price: 295,
        originalPrice: 360,
        rating: 4.9,
        reviewsCount: 35,
        badge: 'FRESH',
        image: 'https://images.unsplash.com/photo-1592945403244-b3fbafd7f539?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Sea Breeze, Crisp Bergamot, Lemon Zest',
        heartNotes: 'Water Lily, Oceanic Accord, Cedar',
        baseNotes: 'Driftwood, White Musk, Vetiver',
        perfumer: 'Notch Riviera Lab',
        description: 'Invigorating oceanic breeze with sun-drenched Mediterranean citrus.',
        sizes: ["100 ml", "50 ml"],
        stock: 100
    },
    {
        id: 'notch-secret-oud',
        code: 'PRD-1014',
        name: 'Notch Secret Oud Parfum',
        subtitle: 'For Men • Intense Aged Oud & Smoky Leather',
        category: 'men',
        collection: 'night',
        scentFamily: 'woody',
        price: 395,
        originalPrice: 480,
        rating: 5.0,
        reviewsCount: 50,
        badge: 'LUXURY',
        image: 'https://images.unsplash.com/photo-1615397349754-cfa2066a298e?auto=format&fit=crop&w=800&q=80',
        topNotes: 'Smoky Oud, Black Pepper, Bergamot',
        heartNotes: 'Incense, Leather, Dark Rose',
        baseNotes: 'Cambodian Oud, Cedarwood, Amber',
        perfumer: 'Master Qatari Perfumers',
        description: 'An intense, mysterious blend of aged Cambodian oud and smoky leather.',
        sizes: ["100 ml"],
        stock: 75
    }
];

const SCENT_FAMILY_INFO = {
    citrus: { title: "Fresh & Citrus", icon: "🍋", desc: "Invigorating notes of Italian Bergamot, Crisp Lemon, and Zesty Mandarin." },
    floral: { title: "Sensual Floral", icon: "🌸", desc: "Romantic blooms of Qatari Jasmine, Damask Rose, and Peony." },
    woody: { title: "Earthy & Woody", icon: "🌲", desc: "Deep grounding aromas of Cedarwood, Vetiver, and Guaiac." },
    oceanic: { title: "Aquatic Breeze", icon: "🌊", desc: "Refreshing marine accents reminiscent of sea mist." },
    amber: { title: "Warm Amber & Spices", icon: "✨", desc: "Opulent blend of Roasted Tonka, Saffron, Vanilla, and Leather." }
};

const PRODUCTS = [...DEFAULT_PRODUCTS];

/**
 * Returns complete product list combining custom Product Master items, API items, and default catalog.
 */
function getProductsList() {
    const list = [];

    // 1. Load Custom Products created in Product Master from localStorage
    const saved = localStorage.getItem('notch_custom_products');
    if (saved) {
        try {
            const parsed = JSON.parse(saved);
            if (Array.isArray(parsed) && parsed.length > 0) {
                parsed.filter(p => p && p.name).forEach(p => {
                    list.push({ ...p });
                });
            }
        } catch (e) {
            console.error('Error reading saved products:', e);
        }
    }

    // 2. Load API Products if loaded into PRODUCTS array
    if (PRODUCTS.length > 0) {
        PRODUCTS.forEach(p => {
            if (p && p.name) {
                if (!list.some(existing => (existing.id && existing.id === p.id) || (existing.name && existing.name === p.name))) {
                    list.push({ ...p });
                }
            }
        });
    }

    // 3. Fall back to DEFAULT_PRODUCTS catalog if missing
    DEFAULT_PRODUCTS.forEach(dp => {
        if (!list.some(existing => (existing.id && existing.id === dp.id) || (existing.name && existing.name === dp.name))) {
            list.push({ ...dp });
        }
    });

    return list;
}

// Auto-sync PRODUCTS array with Database API on load
(async function syncProductsFromApi() {
    try {
        const res = await fetch('/api/products');
        if (res.ok) {
            const data = await res.json();
            if (Array.isArray(data) && data.length > 0) {
                const apiProducts = data.map((p, idx) => ({
                    id: p.id || p.Id || `notch-api-${idx}`,
                    code: p.code || p.ProductCode || p.id || p.Id || `PRD-${100 + idx}`,
                    name: p.name || p.Name,
                    subtitle: p.subtitle || p.Subtitle || p.tagline || p.Tagline || '',
                    category: (p.category || p.Category || 'unisex').toLowerCase(),
                    scentFamily: (p.scentFamily || p.ScentFamily || p.family || p.Family || 'amber').toLowerCase(),
                    price: Number(p.price || p.Price) || 0,
                    originalPrice: Number(p.originalPrice || p.OriginalPrice) || 0,
                    rating: Number(p.rating || p.Rating) || 5.0,
                    reviewsCount: Number(p.reviewsCount || p.ReviewsCount) || 1,
                    badge: p.badge || p.Badge || '',
                    image: p.image || p.Image || '',
                    topNotes: p.topNotes || p.TopNotes || '',
                    heartNotes: p.heartNotes || p.HeartNotes || '',
                    baseNotes: p.baseNotes || p.BaseNotes || '',
                    perfumer: p.perfumer || p.Perfumer || '',
                    description: p.description || p.Description || '',
                    sizes: ["100 ml", "50 ml"],
                    stock: Number(p.stock || p.Stock) || 100
                }));

                PRODUCTS.length = 0;
                PRODUCTS.push(...apiProducts);
            }
        }
    } catch (e) {
        console.warn('API sync fallback note:', e);
    } finally {
        if (typeof renderProducts === 'function') {
            renderProducts(typeof currentFilter !== 'undefined' ? currentFilter : 'all');
        }
        window.dispatchEvent(new CustomEvent('notch_products_ready'));
    }
})();

