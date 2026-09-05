/**
 * Notch Perfumes - Frontend Application Logic (Rose Gold Luxury Theme)
 */

let cart = JSON.parse(localStorage.getItem('notch_cart')) || [
    { productId: 'notch-noura-her', size: '100 ml', quantity: 1 },
    { productId: 'notch-celeste-women', size: '50 ml', quantity: 1 }
];

function getLoggedInUserEmail() {
    if (localStorage.getItem('customerLoggedIn') === 'true') {
        return localStorage.getItem('customerEmail') || 'customer_user';
    }
    if (localStorage.getItem('adminLoggedIn') === 'true') {
        return 'admin_user';
    }
    return null;
}

function getUserWishlistKey() {
    const email = getLoggedInUserEmail();
    if (!email) return null;
    return `notch_wishlist_${email.toLowerCase().replace(/[^a-z0-9]/g, '_')}`;
}

function getWishlist() {
    const key = getUserWishlistKey();
    if (!key) return [];
    try {
        return JSON.parse(localStorage.getItem(key)) || [];
    } catch(e) {
        return [];
    }
}

function saveWishlist(wishlistArray) {
    const key = getUserWishlistKey();
    if (key) {
        localStorage.setItem(key, JSON.stringify(wishlistArray));
    }
}

let currentFilter = 'all';
let currentNoteFilter = 'all';
let currentHeroSlide = 0;
let heroTimer = null;

let quizState = {
    step: 1,
    answers: { gender: '', vibe: '', occasion: '' }
};

function initApp() {
    initHeroSlider();
    renderProducts(typeof currentFilter !== 'undefined' ? currentFilter : 'all');
    renderCart();
    updateHeaderCounters();
    initSearch();
    initEventListeners();
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initApp);
} else {
    initApp();
}

window.addEventListener('load', initApp);

window.addEventListener('notch_products_ready', () => {
    renderProducts(typeof currentFilter !== 'undefined' ? currentFilter : 'all');
});

function initHeroSlider() {
    const slides = document.querySelectorAll('.hero-slide');
    const dotsContainer = document.getElementById('hero-dots');
    
    if (!slides.length || !dotsContainer) return;

    dotsContainer.innerHTML = '';
    slides.forEach((_, idx) => {
        const dot = document.createElement('button');
        dot.className = `w-3 h-3 rounded-full border border-rosegold-400/60 transition-all ${idx === 0 ? 'bg-rosegold-400 w-8' : 'bg-white/40'}`;
        dot.setAttribute('aria-label', `Go to slide ${idx + 1}`);
        dot.onclick = () => setHeroSlide(idx);
        dotsContainer.appendChild(dot);
    });

    startHeroTimer();
}

function setHeroSlide(index) {
    const slides = document.querySelectorAll('.hero-slide');
    const dots = document.querySelectorAll('#hero-dots button');
    
    slides.forEach((slide, idx) => {
        if (idx === index) {
            slide.classList.add('active');
        } else {
            slide.classList.remove('active');
        }
    });

    dots.forEach((dot, idx) => {
        if (idx === index) {
            dot.className = 'w-8 h-3 rounded-full bg-rosegold-400 border border-rosegold-400 transition-all';
        } else {
            dot.className = 'w-3 h-3 rounded-full bg-white/40 border border-rosegold-400/60 transition-all';
        }
    });

    currentHeroSlide = index;
}

function startHeroTimer() {
    if (heroTimer) clearInterval(heroTimer);
    heroTimer = setInterval(() => {
        const slides = document.querySelectorAll('.hero-slide');
        if (!slides.length) return;
        const nextIndex = (currentHeroSlide + 1) % slides.length;
        setHeroSlide(nextIndex);
    }, 5500);
}

function renderProducts(filterCategory = 'all') {
    const grid = document.getElementById('products-grid');
    if (!grid) return;

    const allProds = getProductsList();
    let filtered = allProds;
    if (filterCategory !== 'all') {
        filtered = allProds.filter(p => (p.category || '').toLowerCase() === filterCategory.toLowerCase());
    }

    if (currentNoteFilter !== 'all') {
        filtered = filtered.filter(p => (p.scentFamily || '').toLowerCase() === currentNoteFilter.toLowerCase());
    }

    if (filtered.length === 0) {
        grid.innerHTML = `
            <div class="col-span-full py-16 text-center text-gray-500">
                <p class="text-2xl font-serif-heading italic mb-2">No fragrances found in this category</p>
                <button onclick="resetFilters()" class="text-sm font-medium text-rose-900 underline hover:text-rosegold-600">Reset Filters</button>
            </div>
        `;
        return;
    }

    const activeWishlist = getWishlist();
    grid.innerHTML = filtered.map(product => {
        const isWishlisted = activeWishlist.includes(product.id);
        const ratingVal = product.rating || 5.0;
        const reviewsVal = product.reviewsCount || 1;
        const defaultSize = (product.sizes && product.sizes.length) ? product.sizes[0] : '100 ml';
        const topNotesVal = product.topNotes || 'Agarwood (Oud), Amber, Floral Notes';

        return `
            <div class="product-card bg-white rounded-xl overflow-hidden border border-rose-100 shadow-sm flex flex-col group relative">
                ${product.badge ? `<span class="absolute top-3 left-3 z-10 px-3 py-1 text-[11px] font-semibold tracking-wider uppercase text-rose-950 bg-rosegold-300 rounded-full shadow-sm">${product.badge}</span>` : ''}
                
                <button onclick="toggleWishlist('${product.id}')" class="absolute top-3 right-3 z-10 w-9 h-9 rounded-full bg-white/80 hover:bg-white backdrop-blur-sm shadow flex items-center justify-center text-gray-600 hover:text-red-500 transition-colors" title="Add to Wishlist">
                    <svg class="w-5 h-5 ${isWishlisted ? 'fill-red-500 text-red-500' : 'fill-none stroke-current'}" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                    </svg>
                </button>

                <div class="product-image-container relative bg-rose-50/40 h-72 overflow-hidden cursor-pointer" onclick="openQuickView('${product.id}')">
                    <img src="${(product.image && product.image.trim().length > 5) ? product.image : 'https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80'}" alt="${product.name}" class="w-full h-full object-cover object-center" onerror="this.onerror=null;this.src='https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80';" loading="lazy" />
                    
                    <div class="absolute inset-0 bg-rose-950/20 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                        <span class="px-4 py-2 bg-white/90 backdrop-blur-md text-rose-950 text-xs font-semibold uppercase tracking-wider rounded-full shadow-md transform translate-y-2 group-hover:translate-y-0 transition-transform">
                            Quick View
                        </span>
                    </div>
                </div>

                <div class="p-5 flex-1 flex flex-col justify-between">
                    <div>
                        <div class="flex items-center gap-1 mb-1">
                            <span class="text-xs text-rosegold-600">★</span>
                            <span class="text-xs font-semibold text-gray-800">${ratingVal}</span>
                            <span class="text-xs text-gray-400">(${reviewsVal})</span>
                        </div>
                        
                        <h3 class="font-serif-heading text-xl font-bold text-rose-950 cursor-pointer hover:text-rosegold-600 transition-colors" onclick="openQuickView('${product.id}')">
                            ${product.name}
                        </h3>
                        <p class="text-xs text-gray-500 mt-1 mb-3 line-clamp-1">${product.subtitle || 'Fine Qatari EDP'}</p>

                        <div class="bg-rose-50/60 border border-rose-100 rounded-lg p-2.5 mb-4 text-[11px] text-gray-600 space-y-1">
                            <div class="flex items-center gap-1.5">
                                <span class="font-semibold text-rose-900">Notes:</span>
                                <span class="truncate">${topNotesVal}</span>
                            </div>
                        </div>
                    </div>

                    <div class="pt-2 border-t border-rose-50 flex items-center justify-between">
                        <div>
                            <div class="flex items-baseline gap-2">
                                <span class="text-lg font-bold text-rose-950">QAR ${product.price}</span>
                                ${product.originalPrice ? `<span class="text-xs text-gray-400 line-through">QAR ${product.originalPrice}</span>` : ''}
                            </div>
                            <span class="text-[10px] text-rose-700 font-medium">Taxes Included</span>
                        </div>
                        
                        <button onclick="addToCart('${product.id}', '${defaultSize}', 1)" class="px-4 py-2 bg-rose-950 hover:bg-rose-900 text-rosegold-300 font-bold rounded-lg text-xs tracking-wide transition-all shadow-sm flex items-center gap-1.5">
                            <svg class="w-3.5 h-3.5 fill-none stroke-current" viewBox="0 0 24 24" stroke-width="2"><path d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"/></svg>
                            Add
                        </button>
                    </div>
                </div>
            </div>
        `;
    }).join('');
}

function filterCategory(category, element) {
    currentFilter = category;
    renderProducts(currentFilter);
}

function filterScentNote(scentFamily, element) {
    if (currentNoteFilter === scentFamily) {
        currentNoteFilter = 'all';
    } else {
        currentNoteFilter = scentFamily;
    }
    renderProducts(currentFilter);
}

function resetFilters() {
    currentFilter = 'all';
    currentNoteFilter = 'all';
    renderProducts();
}

function openQuickView(productId) {
    const product = getProductsList().find(p => p.id === productId);
    if (!product) return;

    const modal = document.getElementById('quickview-modal');
    const modalContent = document.getElementById('quickview-content');

    let selectedSize = (product.sizes && product.sizes.length) ? product.sizes[0] : '100 ml';

    modalContent.innerHTML = `
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 p-6 md:p-8">
            <div class="space-y-4">
                <div class="rounded-xl overflow-hidden bg-rose-50/40 h-80 md:h-96 border border-rose-100 shadow-inner">
                    <img id="qv-main-image" src="${product.image}" alt="${product.name}" class="w-full h-full object-cover" />
                </div>
            </div>

            <div class="flex flex-col justify-between">
                <div>
                    <div class="flex items-center justify-between mb-2">
                        <span class="px-2.5 py-0.5 text-[10px] font-semibold uppercase tracking-wider text-rose-900 bg-rosegold-300/40 rounded-full">${product.badge || product.category}</span>
                        <div class="flex items-center gap-1 text-xs text-rosegold-600">
                            ★ <span class="font-bold text-gray-800">${product.rating || 5.0}</span> (${product.reviewsCount || 1} reviews)
                        </div>
                    </div>

                    <h2 class="font-serif-heading text-3xl font-bold text-rose-950 mb-1">${product.name}</h2>
                    <p class="text-xs text-gray-500 mb-4">${product.subtitle || ''}</p>

                    <div class="flex items-baseline gap-3 mb-4 pb-4 border-b border-gray-100">
                        <span class="text-2xl font-bold text-rose-950">QAR ${product.price}</span>
                        ${product.originalPrice ? `<span class="text-sm text-gray-400 line-through">QAR ${product.originalPrice}</span>` : ''}
                    </div>

                    <p class="text-xs text-gray-600 leading-relaxed mb-6">${product.description || ''}</p>

                    <div class="bg-rose-50/60 rounded-xl p-4 border border-rose-100 space-y-2 mb-6 text-xs text-gray-700">
                        <div class="flex"><span class="w-24 font-bold text-rose-900">Top Notes:</span> <span>${product.topNotes || 'Essential oils, Bergamot'}</span></div>
                        <div class="flex"><span class="w-24 font-bold text-rose-900">Heart Notes:</span> <span>${product.heartNotes || 'Floral Essence'}</span></div>
                        <div class="flex"><span class="w-24 font-bold text-rose-900">Base Notes:</span> <span>${product.baseNotes || 'Amber, Musk'}</span></div>
                    </div>
                </div>

                <div class="flex gap-3 pt-4 border-t border-gray-100">
                    <button onclick="addToCart('${product.id}', '${selectedSize}', 1); closeQuickView();" class="flex-1 py-3.5 bg-rose-950 hover:bg-rose-900 text-rosegold-300 font-bold text-sm rounded-xl shadow-md transition-all flex items-center justify-center gap-2">
                        Add to Bag • QAR ${product.price}
                    </button>
                </div>
            </div>
        </div>
    `;

    modal.classList.remove('hidden');
    modal.classList.add('flex');
}

function closeQuickView() {
    const modal = document.getElementById('quickview-modal');
    if (modal) {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    }
}

function isUserLoggedIn() {
    return localStorage.getItem('customerLoggedIn') === 'true' || localStorage.getItem('adminLoggedIn') === 'true';
}

function handleAccountClick(event) {
    if (isUserLoggedIn()) {
        if (event) event.preventDefault();
        const email = localStorage.getItem('customerEmail') || 'User';
        if (confirm(`You are currently logged in as ${email}.\nDo you want to log out?`)) {
            localStorage.removeItem('customerLoggedIn');
            localStorage.removeItem('customerEmail');
            localStorage.removeItem('customerName');
            localStorage.removeItem('adminLoggedIn');
            showToast('Logged out successfully', 'info');
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 600);
        }
    }
}

function addToCart(productId, size = '50 ml', quantity = 1) {
    if (!isUserLoggedIn()) {
        window.location.href = 'login.html';
        return;
    }

    const product = getProductsList().find(p => p.id === productId);
    if (!product) return;

    const existingIndex = cart.findIndex(item => item.productId === productId && item.size === size);
    if (existingIndex > -1) {
        cart[existingIndex].quantity += quantity;
    } else {
        cart.push({ productId, size, quantity });
    }

    saveCart();
    renderCart();
    updateHeaderCounters();
    openCart();
    showToast(`Added <strong>${product.name}</strong> to your bag 🌹`, 'success');
}

function removeFromCart(index) {
    const item = cart[index];
    const product = getProductsList().find(p => p.id === item.productId);
    cart.splice(index, 1);
    saveCart();
    renderCart();
    updateHeaderCounters();
    showToast(`Removed from bag`, 'info');
}

function updateCartQuantity(index, delta) {
    if (cart[index]) {
        cart[index].quantity += delta;
        if (cart[index].quantity <= 0) {
            removeFromCart(index);
            return;
        }
        saveCart();
        renderCart();
        updateHeaderCounters();
    }
}

function saveCart() {
    localStorage.setItem('notch_cart', JSON.stringify(cart));
}

function renderCart() {
    const container = document.getElementById('cart-items-container');
    const subtotalEl = document.getElementById('cart-subtotal');
    const totalEl = document.getElementById('cart-total');
    const freeShippingBar = document.getElementById('free-shipping-progress');
    const freeShippingText = document.getElementById('free-shipping-text');

    if (!container) return;

    if (cart.length === 0) {
        container.innerHTML = `
            <div class="py-16 text-center text-gray-500">
                <p class="font-serif-heading text-xl font-bold text-rose-950 mb-1">Your Shopping Bag is Empty</p>
                <button onclick="closeCart()" class="px-6 py-2.5 bg-rose-950 text-rosegold-300 rounded-lg text-xs font-semibold uppercase">
                    Explore Fragrances
                </button>
            </div>
        `;
        if (subtotalEl) subtotalEl.textContent = 'QAR 0';
        if (totalEl) totalEl.textContent = 'QAR 0';
        if (freeShippingBar) freeShippingBar.style.width = '0%';
        return;
    }

    let subtotal = 0;

    container.innerHTML = cart.map((item, idx) => {
        const product = getProductsList().find(p => p.id === item.productId);
        if (!product) return '';
        const itemTotal = product.price * item.quantity;
        subtotal += itemTotal;

        return `
            <div class="flex gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm relative">
                <img src="${product.image}" alt="${product.name}" class="w-20 h-20 object-cover rounded-lg bg-gray-50 flex-shrink-0" />
                
                <div class="flex-1 flex flex-col justify-between">
                    <div>
                        <div class="flex justify-between items-start gap-2">
                            <h4 class="font-serif-heading font-bold text-rose-950 text-base leading-tight">${product.name}</h4>
                            <button onclick="removeFromCart(${idx})" class="text-gray-400 hover:text-red-500">✕</button>
                        </div>
                        <span class="inline-block text-[11px] text-gray-500 mt-0.5">Size: ${item.size}</span>
                    </div>

                    <div class="flex justify-between items-center mt-3">
                        <div class="flex items-center border border-gray-200 rounded-lg overflow-hidden bg-gray-50">
                            <button onclick="updateCartQuantity(${idx}, -1)" class="w-7 h-7 flex items-center justify-center text-gray-600 font-bold">-</button>
                            <span class="w-8 text-center text-xs font-semibold text-gray-800">${item.quantity}</span>
                            <button onclick="updateCartQuantity(${idx}, 1)" class="w-7 h-7 flex items-center justify-center text-gray-600 font-bold">+</button>
                        </div>

                        <span class="text-sm font-bold text-rose-950">QAR ${itemTotal}</span>
                    </div>
                </div>
            </div>
        `;
    }).join('');

    const freeThreshold = 999;
    const progressPercent = Math.min(100, (subtotal / freeThreshold) * 100);

    if (subtotalEl) subtotalEl.textContent = `QAR ${subtotal}`;
    if (totalEl) totalEl.textContent = `QAR ${subtotal}`;
    if (freeShippingBar) freeShippingBar.style.width = `${progressPercent}%`;
    if (freeShippingText) {
        if (subtotal >= freeThreshold) {
            freeShippingText.innerHTML = `🌹 You qualify for <strong>FREE Delivery</strong>`;
        } else {
            const diff = freeThreshold - subtotal;
            freeShippingText.innerHTML = `Add <strong>QAR ${diff.toLocaleString('en-US')}</strong> more for <strong>FREE Delivery</strong>`;
        }
    }
}

function openCart() {
    const drawer = document.getElementById('cart-drawer');
    if (drawer) drawer.classList.remove('translate-x-full');
}

function closeCart() {
    const drawer = document.getElementById('cart-drawer');
    if (drawer) drawer.classList.add('translate-x-full');
}

function toggleWishlist(productId) {
    if (!isUserLoggedIn()) {
        showToast('🔒 Please log in to save items to your wishlist!', 'warning');
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 1000);
        return;
    }

    let userWishlist = getWishlist();
    const index = userWishlist.indexOf(productId);

    if (index > -1) {
        userWishlist.splice(index, 1);
        showToast(`Removed from wishlist`, 'info');
    } else {
        userWishlist.push(productId);
        showToast(`Added to wishlist ❤️`, 'success');
    }

    saveWishlist(userWishlist);
    updateHeaderCounters();
    if (typeof renderProducts === 'function' && document.getElementById('products-grid')) {
        renderProducts(typeof currentFilter !== 'undefined' ? currentFilter : 'all');
    }
}

function updateHeaderCounters() {
    const cartCountEl = document.getElementById('cart-count-badge');
    const wishlistCountEl = document.getElementById('wishlist-count-badge');

    const totalCartQty = cart.reduce((sum, item) => sum + item.quantity, 0);

    if (cartCountEl) {
        cartCountEl.textContent = totalCartQty;
        cartCountEl.style.display = totalCartQty > 0 ? 'flex' : 'none';
    }

    if (wishlistCountEl) {
        const userWishlist = getWishlist();
        const count = isUserLoggedIn() ? userWishlist.length : 0;
        wishlistCountEl.textContent = count;
        wishlistCountEl.style.display = count > 0 ? 'flex' : 'none';
    }

    const accountLinks = document.querySelectorAll('a[href="login.html"]');
    accountLinks.forEach(link => {
        if (!link.dataset.accountBound) {
            link.dataset.accountBound = 'true';
            link.addEventListener('click', (e) => handleAccountClick(e));
        }
        if (isUserLoggedIn()) {
            const email = localStorage.getItem('customerEmail') || 'User';
            link.title = `Logged in as ${email} (Click to Logout)`;
        } else {
            link.title = 'Account Login';
        }
    });
}

function initSearch() {
    const searchInput = document.getElementById('search-input');
    const searchResults = document.getElementById('search-results');

    if (!searchInput || !searchResults) return;

    searchInput.addEventListener('input', (e) => {
        const query = e.target.value.trim().toLowerCase();
        if (query.length < 2) {
            searchResults.classList.add('hidden');
            return;
        }

        const matches = getProductsList().filter(p => 
            (p.name && p.name.toLowerCase().includes(query)) ||
            (p.topNotes && p.topNotes.toLowerCase().includes(query)) ||
            (p.perfumer && p.perfumer.toLowerCase().includes(query))
        );

        if (matches.length === 0) {
            searchResults.innerHTML = `<div class="p-4 text-xs text-gray-500 text-center">No perfumes matching "${query}"</div>`;
        } else {
            searchResults.innerHTML = matches.map(p => `
                <div onclick="openQuickView('${p.id}'); document.getElementById('search-results').classList.add('hidden');" class="flex items-center gap-3 p-3 hover:bg-rose-50/60 cursor-pointer border-b border-gray-100 last:border-0 transition-colors">
                    <img src="${p.image}" class="w-10 h-10 object-cover rounded bg-gray-100" />
                    <div>
                        <h5 class="text-xs font-bold text-rose-950">${p.name}</h5>
                        <p class="text-[10px] text-gray-500 line-clamp-1">${p.subtitle || ''}</p>
                    </div>
                    <span class="ml-auto text-xs font-bold text-rosegold-600">QAR ${p.price.toLocaleString('en-US')}</span>
                </div>
            `).join('');
        }

        searchResults.classList.remove('hidden');
    });
}

function openQuizModal() {
    quizState = { step: 1, answers: { gender: '', vibe: '', occasion: '' } };
    renderQuizStep();
    const modal = document.getElementById('quiz-modal');
    if (modal) modal.classList.remove('hidden');
}

function closeQuizModal() {
    const modal = document.getElementById('quiz-modal');
    if (modal) modal.classList.add('hidden');
}

function selectQuizAnswer(key, value) {
    quizState.answers[key] = value;
    quizState.step++;
    renderQuizStep();
}

function renderQuizStep() {
    const body = document.getElementById('quiz-modal-body');
    if (!body) return;

    if (quizState.step === 1) {
        body.innerHTML = `
            <div class="text-center py-4">
                <span class="text-xs font-semibold text-rosegold-600 uppercase tracking-widest">Step 1 of 3</span>
                <h3 class="font-serif-heading text-2xl font-bold text-rose-950 mt-1 mb-6">Who are you shopping for?</h3>
                <div class="grid grid-cols-3 gap-4 max-w-md mx-auto">
                    <button onclick="selectQuizAnswer('gender', 'men')" class="p-5 border-2 border-stone-200 hover:border-rosegold-400 hover:bg-rose-50/50 rounded-2xl flex flex-col items-center">
                        <span class="text-3xl mb-2">🤵</span>
                        <span class="text-xs font-bold text-rose-950 uppercase">For Him</span>
                    </button>
                    <button onclick="selectQuizAnswer('gender', 'women')" class="p-5 border-2 border-stone-200 hover:border-rosegold-400 hover:bg-rose-50/50 rounded-2xl flex flex-col items-center">
                        <span class="text-3xl mb-2">💃</span>
                        <span class="text-xs font-bold text-rose-950 uppercase">For Her</span>
                    </button>
                    <button onclick="selectQuizAnswer('gender', 'unisex')" class="p-5 border-2 border-stone-200 hover:border-rosegold-400 hover:bg-rose-50/50 rounded-2xl flex flex-col items-center">
                        <span class="text-3xl mb-2">✨</span>
                        <span class="text-xs font-bold text-rose-950 uppercase">Unisex</span>
                    </button>
                </div>
            </div>
        `;
    } else {
        let bestMatch = getProductsList().find(p => p.category === quizState.answers.gender) || getProductsList()[0];
        body.innerHTML = `
            <div class="text-center py-6 max-w-md mx-auto">
                <span class="px-3 py-1 bg-rose-100 text-rose-900 text-[10px] font-bold uppercase rounded-full">Scent Match</span>
                <h3 class="font-serif-heading text-2xl font-bold text-rose-950 mt-2 mb-4">${bestMatch.name}</h3>
                <div class="flex gap-3">
                    <a href="checkout.html" class="block w-full py-4 text-center bg-rose-950 hover:bg-rose-900 text-rosegold-300 font-bold text-xs uppercase tracking-widest rounded-xl transition-all shadow-md">🔒 Proceed to Purchase & Checkout</a>
                </div>
            </div>
        `;
    }
}

function showToast(message, type = 'info') {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast px-4 py-3 rounded-xl shadow-xl backdrop-blur-md border text-xs font-medium flex items-center gap-3 text-rose-950 bg-white/95 border-rose-950/10 min-w-[260px]`;

    toast.innerHTML = `
        <span class="text-lg">${type === 'success' ? '🌹' : 'ℹ️'}</span>
        <div class="flex-1">${message}</div>
    `;

    container.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(10px)';
        toast.style.transition = 'all 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3200);
}

function initEventListeners() {
    const mobileBtn = document.getElementById('mobile-menu-btn');
    const mobileMenu = document.getElementById('mobile-menu');

    if (mobileBtn && mobileMenu) {
        mobileBtn.addEventListener('click', () => {
            mobileMenu.classList.toggle('hidden');
        });
    }
}
