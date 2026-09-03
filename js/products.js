/**
 * Notch Perfumes - Product Catalog Data
 */
const PRODUCTS = [
    {
        id: "notch-raw-men",
        name: "Notch Raw Eau De Parfum",
        subtitle: "For Men • Fresh & Citrus Woody",
        category: "men",
        scentFamily: "citrus",
        price: 119,
        originalPrice: 139,
        rating: 4.8,
        reviewsCount: 420,
        badge: "Bestseller",
        image: "https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Bergamot, Lemon, Crisp Watery Accord",
        heartNotes: "Violet Leaves, Geranium, Lavender",
        baseNotes: "Guaiac Wood, Patchouli, Cashmeran",
        perfumer: "Olivier Pescheux (Qatar)",
        description: "Notch Raw draws inspiration from rain washing over lush foliage. A vibrant blend of citrus top notes paired with rich woody undertones for the dynamic, confident man.",
        sizes: ["50 ml", "100 ml", "20 ml Pocket"]
    },
    {
        id: "notch-celeste-women",
        name: "Notch Celeste Eau De Parfum",
        subtitle: "For Women • Elegant Floral Amber",
        category: "women",
        scentFamily: "floral",
        price: 119,
        originalPrice: 139,
        rating: 4.9,
        reviewsCount: 512,
        badge: "Bestseller",
        image: "https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1588405748880-12d1d2a59f75?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Mandarin, Green Pear, Grapefruit, Peach",
        heartNotes: "Jasmine, Sambac, Waterlily, Orange Blossom",
        baseNotes: "White Musk, Patchouli, Sandalwood, Amber",
        perfumer: "Harry Fremont (Qatar)",
        description: "Notch Celeste evokes the carefree joy of a sunny spring afternoon. Vibrant fruity accents blend seamlessly into a rich heart of jasmine and white florals.",
        sizes: ["50 ml", "100 ml", "20 ml Pocket"]
    },
    {
        id: "notch-amalfi-bleue",
        name: "Notch Amalfi Bleue EDP",
        subtitle: "Unisex • Mediterranean Aquatic Fresh",
        category: "unisex",
        scentFamily: "oceanic",
        price: 139,
        originalPrice: 169,
        rating: 4.9,
        reviewsCount: 380,
        badge: "Trending",
        image: "https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80",
            "https://images.unsplash.com/photo-1616949755610-8c9bbc08f138?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Citrus Zest, Apple, Sea Breeze Accord",
        heartNotes: "Clary Sage, Violet Leaf, Fig Tree",
        baseNotes: "Ambergris, Driftwood, Vetiver",
        perfumer: "Jordi Fernandez (Qatar)",
        description: "Transport yourself to the sun-drenched cliffs of the Italian coastline. Fresh ocean breezes meet aromatic Mediterranean herbs and warm driftwood notes.",
        sizes: ["90 ml", "20 ml Travel"]
    },
    {
        id: "notch-steele-men",
        name: "Notch Steele Eau De Parfum",
        subtitle: "For Men • Intense Spiced Leather & Wood",
        category: "men",
        scentFamily: "woody",
        price: 119,
        originalPrice: 129,
        rating: 4.7,
        reviewsCount: 295,
        badge: "Popular",
        image: "https://images.unsplash.com/photo-1508746829417-e6f548d8d6ed?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1508746829417-e6f548d8d6ed?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Pink Pepper, Bergamot, Cardamom",
        heartNotes: "Nutmeg, Pimento, Cistus",
        baseNotes: "Smoky Leather, Cedarwood, Vanilla",
        perfumer: "Fabrice Pellegrin (Qatar)",
        description: "Notch Steele embodies charisma and strength. A warm, spicy heart layered with opulent leather and smoky woods leaves an undeniable impression.",
        sizes: ["50 ml", "100 ml"]
    },
    {
        id: "notch-nox-him",
        name: "Notch Nox Eau De Parfum",
        subtitle: "For Men • Dark Amber & Vanilla Spices",
        category: "men",
        scentFamily: "amber",
        price: 169,
        originalPrice: 189,
        rating: 4.9,
        reviewsCount: 184,
        badge: "Luxury Line",
        image: "https://images.unsplash.com/photo-1615397349754-cfa2066a298e?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1615397349754-cfa2066a298e?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Black Pepper, Roasted Coffee, Saffron",
        heartNotes: "Cinnamon Bark, Incense, Dark Rose",
        baseNotes: "Rich Amber, Tonka Bean, Oud Accord",
        perfumer: "Nathalie Lorson (Qatar)",
        description: "Created for moonlit evenings and high-society galas. Notch Nox blends dark spice, rich coffee notes, and velvety amber for an enchanting nocturnal aura.",
        sizes: ["100 ml"]
    },
    {
        id: "notch-noura-her",
        name: "Notch Noura Eau De Parfum",
        subtitle: "For Women • Opulent Rose & Soft Musk",
        category: "women",
        scentFamily: "floral",
        price: 169,
        originalPrice: 189,
        rating: 4.9,
        reviewsCount: 210,
        badge: "Luxury Line",
        image: "https://images.unsplash.com/photo-1588405748880-12d1d2a59f75?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1588405748880-12d1d2a59f75?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Mandarin, Red Berries, Almond Blossom",
        heartNotes: "Damask Rose, Iris, Magnolia",
        baseNotes: "Cashmere Wood, Vanilla Pod, Crystal Musk",
        perfumer: "Michel Girard (Qatar)",
        description: "Notch Noura is the definition of timeless elegance. A sensual rose bloom wrapped in golden vanilla and creamy cashmere woods.",
        sizes: ["100 ml"]
    },
    {
        id: "notch-discovery-kit",
        name: "Notch Discovery Gift Box",
        subtitle: "Unisex • 5 x 5ml Fragrance Miniatures",
        category: "discovery",
        scentFamily: "citrus",
        price: 45,
        originalPrice: 59,
        rating: 4.9,
        reviewsCount: 890,
        badge: "Must Try",
        image: "https://images.unsplash.com/photo-1547887537-6158d64c35b3?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1547887537-6158d64c35b3?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Assorted Top Notes (Citrus, Marine, Floral, Spice)",
        heartNotes: "Curated Qatari Essences",
        baseNotes: "Long-lasting Amber & Wood bases",
        perfumer: "Master Qatari Perfumers",
        description: "Unsure of your signature scent? Explore Notch's top icons (Raw, Steele, Celeste, Nude, Amalfi) in travel-ready miniature bottles.",
        sizes: ["5 x 5ml Set"]
    },
    {
        id: "notch-nude-women",
        name: "Notch Nude Eau De Parfum",
        subtitle: "For Women • Romantic Lychee & Powdery Rose",
        category: "women",
        scentFamily: "floral",
        price: 119,
        originalPrice: 139,
        rating: 4.7,
        reviewsCount: 310,
        badge: "Fan Favorite",
        image: "https://images.unsplash.com/photo-1592945403244-b3fbafd7f539?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1592945403244-b3fbafd7f539?auto=format&fit=crop&w=800&q=80"
        ],
        topNotes: "Lychee, Bergamot, Raspberry",
        heartNotes: "Rose Petals, Violet, Peony",
        baseNotes: "Sandalwood, Vanilla, White Musk",
        perfumer: "Alberto Morillas (Qatar)",
        description: "Notch Nude captures pure romance. Soft fruity nuances lead into a dreamlike heart of fresh garden roses and velvet musk.",
        sizes: ["50 ml", "100 ml"]
    }
];

const SCENT_FAMILY_INFO = {
    citrus: { title: "Fresh & Citrus", icon: "🍋", desc: "Invigorating notes of Italian Bergamot, Crisp Lemon, and Zesty Mandarin." },
    floral: { title: "Sensual Floral", icon: "🌸", desc: "Romantic blooms of Qatari Jasmine, Damask Rose, and Peony." },
    woody: { title: "Earthy & Woody", icon: "🌲", desc: "Deep grounding aromas of Cedarwood, Vetiver, and Guaiac." },
    oceanic: { title: "Aquatic Breeze", icon: "🌊", desc: "Refreshing marine accents reminiscent of sea mist." },
    amber: { title: "Warm Amber & Spices", icon: "✨", desc: "Opulent blend of Roasted Tonka, Saffron, Vanilla, and Leather." }
};
