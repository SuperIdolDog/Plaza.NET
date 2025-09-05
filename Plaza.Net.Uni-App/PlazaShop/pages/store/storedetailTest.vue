<template>
  <view class="profile-container">
    <!-- 骨架屏 -->
    <view v-if="loading">
      <u-skeleton rows="3" title :loading="loading" height="auto" />
    </view>

    <block v-else>
      <!-- 商户信息卡片 -->
      <view class="merchant-card">
        <!-- 轮播图（后端未返回则隐藏） -->
        <u-swiper
          v-if="swiperImages.length"
          :list="swiperImages"
          height="300"
          :autoplay="true"
          interval="3000"
          radius="0"
          indicatorStyle="bottom: 20rpx; active-color: #fff; inactive-color: rgba(255,255,255,0.5);"
        />

        <view class="merchant-header">
          <view class="logo-wrapper">
            <u-avatar
              :src="merchantInfo.logo"
              size="90"
              :fade="true"
              customStyle="border: 4px solid #fff; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"
            />
          </view>

          <view class="base-info">
            <u-text
              class="merchant-name"
              :text="merchantInfo.name"
              bold
              align="center"
              size="18"
            />
            <scroll-view scroll-x class="tags-scroll">
              <view class="tags-container">
                <u-tag
                  v-for="(tag, index) in merchantInfo.tags"
                  :key="index"
                  :text="tag"
                  size="mini"
                  type="error"
                  shape="circle"
                  plain
                  class="tag-item"
                />
              </view>
            </scroll-view>
            <u-text
              class="description u-m-t-10"
              :text="merchantInfo.description || '暂无商户简介'"
              size="13"
              color="#666"
              align="center"
            />
          </view>

          <view class="action-buttons">
            <u-button
              shape="circle"
              size="mini"
              text="联系商家"
              type="error"
              @click="contactMerchant"
            />
          </view>
        </view>

        <!-- 统计 -->
        <u-grid :col="3" :border="false" class="stats-grid">
          <u-grid-item
            v-for="(item, index) in statsData"
            :key="index"
            class="stats-item"
          >
            <u-text
              align="center"
              class="stat-num"
              :text="item.value"
              bold
              size="16"
            />
            <u-text
              align="center"
              class="stat-label u-m-t-4"
              :text="item.label"
              size="12"
              color="#999"
            />
          </u-grid-item>
        </u-grid>

        <!-- 营业信息 -->
        <view class="business-info">
          <view v-if="merchantInfo.address" class="info-item">
            <u-icon name="map" size="20" color="#FF2442" class="u-m-r-4" />
            <u-text class="info-label" :text="merchantInfo.address" size="14" color="#333" />
            <u-icon name="cut" size="18" color="#999" @click="copyAddress" />
          </view>
          <view v-if="merchantInfo.businessHours" class="info-item">
            <u-icon name="clock" size="20" color="#FF2442" class="u-m-r-4" />
            <u-text class="info-label" :text="merchantInfo.businessHours" size="14" color="#333" />
          </view>
        </view>
      </view>

      <!-- 商品分类 -->
      <scroll-view scroll-x class="category-scroll">
        <view class="category-list">
          <u-tag
            v-for="(category, index) in categories"
            :key="category.id"
            :text="category.name"
            size="medium"
            :type="currentCategory === category.id ? 'error' : 'info'"
            :plain="currentCategory !== category.id"
            @click="switchCategory(category.id)"
            class="category-tag"
          />
        </view>
      </scroll-view>

      <!-- 商品列表 -->
      <view v-if="filteredProducts.length" class="product-grid--2col">
        <view
          v-for="item in filteredProducts"
          :key="item.id"
          class="product-card--2col"
          @click="viewProductDetail(item)"
        >
          <view class="card-img-wrapper">
            <image :src="item.cover" mode="aspectFill" lazy-load class="card-img" />
          </view>
          <view class="card-content">
            <u-text :text="item.name" size="14" lines="2" class="u-line-2 u-font-bold" />
            
          </view>
        </view>
      </view>

      <u-empty v-else mode="list" text="暂无商品" color="#999" class="u-m-t-40" />
    </block>

    <u-back-top :scroll-top="scrollTop" icon="arrow-up" top="600" />
  </view>
</template>

<script>
export default {
  data() {
    return {

      loading: true,
      scrollTop: 0,
      storeId: null,
      currentCategory: 'all',
      categories: [{ id: 'all', name: '全部' }],
      merchantInfo: {
        logo: '',
        name: '',
        tags: [],
        description: '',
        swiperImages: [],
        productCount: 0,
        serviceCount: 0,
        rating: '0',
        address: '',
        businessHours: '',
        phone: ''
      },
      productList: []
    };
  },
  computed: {
	   swiperImages() {
	      // 若后端未返回，则返回空数组，避免 undefined.length
	      return Array.isArray(this.merchantInfo.swiperImages)
	        ? this.merchantInfo.swiperImages
	        : ['/static/swiper-background-1.png',
						'/static/swiper-background-2.png',
						'/static/swiper-background-3.png'];
	    },
    statsData() {
      return [
        { value: this.formatNumber(this.merchantInfo.productCount), label: '商品' },
        { value: this.formatNumber(this.merchantInfo.serviceCount), label: '服务' },
        { value: this.merchantInfo.rating, label: '评分' }
      ];
    },
    filteredProducts() {
       return this.currentCategory === 'all'
            ? this.productList
            : this.productList.filter(p => p.productTypeId === this.currentCategory);
    }
   
  },
  async onLoad(options) {
    this.storeId = options.id || 1;
    await Promise.all([
      this.loadStoreInfo(),
      this.loadCategories(),
      this.loadProducts()
    ]);
    this.loading = false;
  },
  onPageScroll(e) {
    this.scrollTop = e.scrollTop;
  },
  methods: {
	  
    /* 店铺信息 */
    async loadStoreInfo() {
      const { data } = await uni.request({
        url: `/api/store/storeInfo/${this.storeId}`,
        method: 'GET'
      });
	   uni.setStorageSync('CURRENT_STORE', {
	       id: data.id,
	        name: data.name,
	        address: data.location || '暂无地址',
	        phone: data.contact || '暂无电话',
	        businessHours: data.businessHours || '暂无营业时间',
	        rating: data.rating || 0,
	        notice: data.notice || '暂无公告',
	        logo: data.imageUrl || '/static/logo.png'
	    });
      this.merchantInfo = {
        logo: data.imageUrl || '/static/logo.png',
        name: data.name,
        tags: [data.category || '店铺'],
        description: data.description,
        address: data.location,
        businessHours: data.businessHours,
        phone: data.contact,
        productCount: 128, // 如需真实数据需后端扩展字段
        serviceCount: 12,
        rating: '4.9'
      };
    },
    /* 商品类型 */
    async loadCategories() {
      const { data } = await uni.request({
        url: `/api/good/store/${this.storeId}/productTypes`,
        method: 'GET'
      });
      this.categories = [
        { id: 'all', name: '全部' },
        ...data.map(t => ({ id: t.id.toString(), name: t.name }))
      ];
    },
    /* 商品列表（示例：/api/store/{id}/products） */
    async loadProducts(reset=true) {
       if (reset) {
          this.page = 1;
          this.hasMore = true;
        }
      
        // 参数统一用 this.currentCategory
        const params = {
          page: this.page,
          pageSize: 6
        };
        if (this.currentCategory !== 'all') {
          params.productTypeId = this.currentCategory;
        }
      
        const { data } = await uni.request({
          url: `/api/good/store/${this.storeId}/products`,
          method: 'GET',
          data: params
        });
      
        this.productList = reset ? data : [...this.productList, ...data];
        this.hasMore = data.length === 6;
        this.page += 1; 
    },
    formatNumber(num) {
      if (num >= 10000) return (num / 10000).toFixed(1) + 'w';
      if (num >= 1000) return (num / 1000).toFixed(1) + 'k';
      return num;
    },
    switchCategory(categoryId) {
      this.currentCategory = categoryId;
	  this.loadProducts(true);
      uni.pageScrollTo({ scrollTop: 0, duration: 200 });
    },
    viewProductDetail(product) {
      uni.navigateTo({ url: `/pages/product/detail?id=${product.id}` });
    },
    contactMerchant() {
      uni.showActionSheet({
        itemList: [`拨打电话: ${this.merchantInfo.phone}`, '复制电话号码'],
        success: res => {
          if (res.tapIndex === 0) {
            uni.makePhoneCall({ phoneNumber: this.merchantInfo.phone });
          } else {
            uni.setClipboardData({
              data: this.merchantInfo.phone,
              success: () => uni.showToast({ title: '已复制', icon: 'success' })
            });
          }
        }
      });
    },
    copyAddress() {
      uni.setClipboardData({
        data: this.merchantInfo.address,
        success: () => uni.showToast({ title: '地址已复制', icon: 'success' })
      });
    }
  }
};
</script>

<style lang="scss" scoped>
/* ===== 极光白紫 - 零变量 ===== */
page {
  background-color: #f8fafc;
}

.profile-container {
  background-color: #f8fafc;
  padding-bottom: 30rpx;
}

/* 商户卡片 - 紫银描边 + 微光阴影 */
.merchant-card {
  margin: 20rpx;
  background-color: #ffffff;
  border-radius: 24rpx;
  overflow: hidden;
  box-shadow: 0 8rpx 32rpx rgba(124, 58, 237, 0.08);
  border: 1rpx solid rgba(124, 58, 237, 0.12);
}

.merchant-header {
  position: relative;
  padding: 0 30rpx 30rpx;
  margin-top: -45rpx;

  .logo-wrapper {
    display: flex;
    justify-content: center;
    margin-bottom: 20rpx;
  }

  .base-info {
    display: flex;
    flex-direction: column;
    align-items: center;

    .merchant-name {
      margin-bottom: 6rpx;
      color: #1e293b;
      font-weight: 600;
    }

    .tags-scroll {
      white-space: nowrap;

      .tags-container {
        display: inline-flex;
        padding: 8rpx 20rpx;
        flex-wrap: nowrap;
      }
    }

    .description {
      padding: 0 30rpx;
      color: #64748b;
    }
  }

  .action-buttons {
    position: absolute;
    top: 10rpx;
    right: 30rpx;
  }
}

/* 统计栏 - 紫银渐变 */
.stats-grid {
  display: flex;
  justify-content: center;
  padding: 24rpx 0;
  border-top: 1rpx solid #f1f5f9;
  background: linear-gradient(135deg, rgba(124, 58, 237, 0.06) 0%, rgba(168, 85, 247, 0.04) 100%);

  .stats-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }
}

/* 营业信息 - 图标主色 */
.business-info {
  padding: 10rpx 0;
  border-top: 1rpx solid #f1f5f9;

  .info-item {
    display: flex;
    align-items: center;
    padding: 16rpx 30rpx;

    .info-label {
      flex: 1;
      margin-right: 16rpx;
      color: #1e293b;
    }
  }
}

/* 分类标签 - 紫银描边 */
.category-scroll {
  white-space: nowrap;
  overflow: hidden;
  background: #fff;
  padding: 16rpx 0;
  margin-bottom: 16rpx;

  .category-list {
    display: inline-flex;
    padding: 0 10rpx;
    flex-wrap: nowrap;
  }

  .category-tag {
    margin-right: 12rpx;
    border-radius: 20rpx;
    background: #f1f5f9;
    border: 1rpx solid rgba(124, 58, 237, 0.2);
    color: #64748b;
    &.u-tag--error {
      background: linear-gradient(135deg, #7c3aed 0%, #a855f7 100%);
      color: #fff;
      border: none;
    }
  }
}

/* 商品网格 - 紫银卡片 */
.product-grid--2col {
  display: flex;
  flex-wrap: wrap;
  padding: 20rpx 10rpx;
}

.product-card--2col {
  width: 50%;
  padding: 0 10rpx 20rpx;
  box-sizing: border-box;

  .card-img-wrapper {
    width: 100%;
    height: 320rpx;
    border-radius: 16rpx 16rpx 0 0;
    overflow: hidden;
    background-color: #f1f5f9;
    box-shadow: inset 0 0 8rpx rgba(124, 58, 237, 0.08);
  }

  .card-img {
    width: 100%;
    height: 100%;
    border-radius: 16rpx 16rpx 0 0;
  }

  .card-content {
    padding: 16rpx;
    background-color: #fff;
    border-radius: 0 0 16rpx 16rpx;
    border: 1rpx solid rgba(124, 58, 237, 0.08);
    border-top: none;
  }

  .price-row {
    display: flex;
    align-items: center;
    margin-top: 8rpx;
  }

  &:active {
    transform: scale(0.97);
    transition: none;
  }
}

/* 加载更多 - 紫银微光 */
.load-more {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20rpx 0;
  color: #94a3b8;
}
</style>