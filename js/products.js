/**
 * Notch Perfumes - Product Catalog Data (Dynamic DB Sync)
 */
const PRODUCTS = [];

const SCENT_FAMILY_INFO = {
    citrus: { title: "Fresh & Citrus", icon: "🍋", desc: "Invigorating notes of Italian Bergamot, Crisp Lemon, and Zesty Mandarin." },
    floral: { title: "Sensual Floral", icon: "🌸", desc: "Romantic blooms of Qatari Jasmine, Damask Rose, and Peony." },
    woody: { title: "Earthy & Woody", icon: "🌲", desc: "Deep grounding aromas of Cedarwood, Vetiver, and Guaiac." },
    oceanic: { title: "Aquatic Breeze", icon: "🌊", desc: "Refreshing marine accents reminiscent of sea mist." },
    amber: { title: "Warm Amber & Spices", icon: "✨", desc: "Opulent blend of Roasted Tonka, Saffron, Vanilla, and Leather." }
};

const DUMMY_IDS = ['notch-oud-royale', 'notch-raw-men', 'notch-celeste-women', 'notch-amalfi-bleue', 'notch-steele-men', 'notch-nox-him', 'notch-noura-her', 'notch-discovery-kit', 'notch-nude-women'];

/**
 * Returns products list (only user created products from DB / Product Master)
 */
function getProductsList() {
    const saved = localStorage.getItem('notch_custom_products');
    if (saved) {
        try {
            const parsed = JSON.parse(saved);
            if (Array.isArray(parsed)) {
                return parsed.filter(p => !DUMMY_IDS.includes(p.id || p.Id));
            }
        } catch (e) {
            console.error('Error reading saved products:', e);
        }
    }
    return PRODUCTS.filter(p => !DUMMY_IDS.includes(p.id || p.Id));
}

// Purge any old dummy items from localStorage immediately
(function purgeDummyItems() {
    try {
        const saved = localStorage.getItem('notch_custom_products');
        if (saved) {
            const parsed = JSON.parse(saved);
            if (Array.isArray(parsed)) {
                const clean = parsed.filter(p => !DUMMY_IDS.includes(p.id || p.Id));
                localStorage.setItem('notch_custom_products', JSON.stringify(clean));
            }
        }
    } catch(e) {}
})();

// Auto-sync PRODUCTS array with Database API on load
(async function syncProductsFromApi() {
    try {
        const res = await fetch('/api/products');
        if (res.ok) {
            const data = await res.json();
            if (Array.isArray(data)) {
                const apiProducts = data
                    .filter(p => !DUMMY_IDS.includes(p.id || p.Id))
                    .map(p => ({
                        id: p.id || p.Id,
                        code: p.id || p.Id,
                        name: p.name || p.Name,
                        subtitle: p.subtitle || p.Subtitle || p.tagline || p.Tagline || '',
                        category: p.category || p.Category || 'men',
                        scentFamily: p.scentFamily || p.ScentFamily || p.family || p.Family || 'amber',
                        price: p.price || p.Price || 0,
                        originalPrice: p.originalPrice || p.OriginalPrice || 0,
                        rating: p.rating || p.Rating || 5.0,
                        reviewsCount: p.reviewsCount || p.ReviewsCount || p.reviewCount || p.ReviewCount || 0,
                        badge: p.badge || p.Badge || '',
                        image: p.image || p.Image || '',
                        topNotes: p.topNotes || p.TopNotes || '',
                        heartNotes: p.heartNotes || p.HeartNotes || '',
                        baseNotes: p.baseNotes || p.BaseNotes || '',
                        perfumer: p.perfumer || p.Perfumer || '',
                        description: p.description || p.Description || '',
                        stock: p.stock || p.Stock || 100
                    }));

                PRODUCTS.length = 0;
                PRODUCTS.push(...apiProducts);
                localStorage.setItem('notch_custom_products', JSON.stringify(apiProducts));

                if (typeof renderProducts === 'function') {
                    renderProducts(typeof currentFilter !== 'undefined' ? currentFilter : 'all');
                }
            }
        }
    } catch (e) {
        console.warn('API sync fallback note:', e);
    }
})();    } catch (e) {}
    }
})();

