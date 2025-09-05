<template>
  <view class="page">
    <scroll-view scroll-y class="content safe-bottom"
                 :style="{ paddingBottom: bottomPadding + 'px' }">

      <!-- 轮播图 -->
      <view class="banner">
        <u-swiper :list="slides" height="700rpx"
                  indicator circular :autoplay="true"
                  radius="0rpx 0rpx 30rpx 30rpx"
                  @click="preview" />
      </view>

      <!-- 商品信息 -->
      <view class="info">
        <u-text :text="goods.title" bold size="32rpx" block margin="0 0 15rpx" />
        <view class="price">
          <u-text :text="goods.price" mode="price" type="error"
                  size="36rpx" bold block />
        </view>
      </view>

      <!-- 分享 -->
      <view class="share" @tap="shareShow = true">
        <u-icon name="share-fill" color="#fff" size="28rpx" />
        <u-text text="分享领随机减免红包" color="#fff" size="26rpx" block />
        <u-icon name="arrow-right" color="#fff" size="24rpx" />
      </view>

      <!-- 促销活动 -->
      <view class="card promo-service">
        <u-text text="促销活动" bold size="28rpx" block margin="0 0 15rpx" />
        <view class="bd">
          <view v-if="promos.length" class="tag-list">
            <view v-for="(p, i) in promos" :key="i" class="tag-item">
              <u-text :text="p.tag" type="error" plain size="mini" />
              <u-text :text="p.txt" size="26rpx" color="#333" />
            </view>
          </view>
          <view v-else class="empty-tip">
            <u-text text="暂无促销活动" color="#999" size="26rpx" />
          </view>
        </view>
      </view>

      <!-- 服务保障 -->
      <view class="card promo-service">
        <u-text text="服务保障" bold size="28rpx" block margin="0 0 15rpx" />
        <view class="bd">
          <view v-if="services.length" class="tag-list">
            <view v-for="(s, i) in services" :key="i" class="tag-item">
              <u-text :text="s.tag" type="error" plain size="mini" />
              <u-text :text="s.txt" size="26rpx" color="#333" />
            </view>
          </view>
          <view v-else class="empty-tip">
            <u-text text="暂无特殊保障" color="#999" size="26rpx" />
          </view>
        </view>
      </view>

      <!-- 用户评价 -->
      <view class="card">
        <u-text text="用户评价" bold size="28rpx" display="inline-block" />
        <u-text :text="`(${reviews.length})`" color="#666"
                size="24rpx" display="inline-block" />
        <view v-if="reviews && reviews.length">
          <view v-for="(review, index) in reviews" :key="index"
                class="comment">
            <u-avatar :src="review.userId" size="60" />
            <view class="body">
              <u-text :text="review.userName" bold block margin="0 0 8rpx" />
              <u-rate :value="review.rating" readonly size="24rpx" />
              <u-text :text="review.content" size="26rpx" lines="3"
                      block margin="8rpx 0" />
              <u-text :text="`购买配置：${review.skusSpecNames} · ${review.createTime}`"
                      color="#999" size="22rpx" block />
            </view>
          </view>
        </view>
        <view v-else class="empty-tip">
          <u-text text="暂无评价" color="#999" size="26rpx" />
        </view>
      </view>
    </scroll-view>

    <!-- 底部操作栏 -->
    <view class="bottom-bar safe-bottom">
      <view class="action" @tap="toHome">
        <u-icon name="home" size="48rpx" color="#666" />
        <u-text text="首页" color="#666" size="22rpx" />
      </view>
      <view class="action" @tap="toCart">
        <u-icon name="shopping-cart" size="48rpx" color="#666" />
        <u-text text="购物车" color="#666" size="22rpx" />
      </view>
      <view class="action" @tap="toStore">
        <u-icon name="home-fill" size="48rpx" color="#666" />
        <u-text text="店铺" color="#666" size="22rpx" />
      </view>

      <view class="btns">
        <!-- 加入购物车 -->
        <u-button type="warning" text="加入购物车" shape="circle"
                  custom-style="height:72rpx;width:200rpx"
                  @tap="openSpec(true)" />
        <!-- 立即购买 -->
        <u-button type="error" text="立即购买" shape="circle"
                  custom-style="height:72rpx;width:200rpx"
                  @tap="openSpec(false)" />
      </view>
    </view>

    <!-- 规格弹窗 -->
    <u-popup :show="specShow" mode="bottom" round="20rpx" closeable
             @close="specShow = false">
      <view class="spec-panel">
        <view class="spec-top">
          <image :src="goods.img" style="width:140rpx;height:140rpx;
                                        border-radius:8rpx" />
          <view style="margin-left:15rpx">
            <u-text :text="currentSku ? currentSku.price : goods.price"
                    mode="price" type="error" size="34rpx" bold
                    block margin="0 0 6rpx" />
            <u-text :text="'库存：' +
                          (currentSku ? currentSku.stockQuantity
                                      : goods.stockQuantity) +
                          ' 件'"
                    color="#666" size="24rpx" block margin="0 0 6rpx" />
            <u-text :text="'已选：' + (selectedSpec || '请选择')"
                    type="error" size="24rpx" block />
          </view>
        </view>

        <!-- 规格列表 -->
        <block v-for="(g, i) in specs" :key="i">
          <u-text :text="g.name" bold size="26rpx"
                  block margin="20rpx 0 10rpx" />
          <view class="spec-list">
            <view v-for="(c, j) in g.values" :key="j"
                  :class="['spec-item', { 'spec-item--active': c.selected }]"
                  @tap="choose(i, j)">
              <u-text :text="c.name" />
            </view>
          </view>
        </block>

        <!-- 数量选择 -->
        <view class="spec-bottom">
          <u-text text="数量" size="26rpx" />
          <u-number-box v-model="quantity" :min="1" :max="99" />
        </view>

        <!-- 弹窗确定按钮 -->
        <u-button type="error" shape="circle"
                  style="width:100%;height:90rpx;font-size:30rpx"
                  @click="isAddCart ? confirmAddCart() : confirmBuy()">
          确定
        </u-button>
      </view>
    </u-popup>

    <!-- 分享弹窗 -->
    <u-popup :show="shareShow" mode="bottom" round="20rpx" closeable
             @close="shareShow = false">
      <view class="share-panel">
        <u-text text="分享到" bold size="28rpx" align="center"
                block margin="0 0 30rpx" />
        <u-grid :col="4" :border="false">
          <u-grid-item v-for="(s, i) in shareList" :key="i"
                       @tap="shareTo(s.txt)">
            <u-icon :name="s.icon" size="60rpx" color="#07c160" />
            <u-text :text="s.txt" margin="10rpx 0 0" align="center" />
          </u-grid-item>
        </u-grid>
        <u-button plain hair-line
                  style="width:100%;height:80rpx;font-size:28rpx;margin-top:20rpx"
                  @tap="shareShow = false">
          取消
        </u-button>
      </view>
    </u-popup>
  </view>
</template>

<script>
export default {
  data() {
    return {
      slides: [],
      goods: {
        id: 0,
        title: '',
        price: 0,
        stockQuantity: 0,
        img: ''
      },
      reviews: [],
      totalReviews: 0,
      pageIndex: 1,
      pageSize: 2,

      skus: [],          // 后端返回的全部 SKU
      hasSpec: true,     // 是否有规格
      currentSku: null,  // 当前选中的 SKU

      promos: [],
      services: [],
      descImgs: [],
      shareList: [],
      specs: [],

      quantity: 1,
      specShow: false,
      shareShow: false,

      isAddCart: false,  // true = 加入购物车  false = 立即购买
      bottomPadding: 100
    };
  },

  computed: {
    selectedSpec() {
      return this.specs
        .map(g => g.values.find(v => v.selected)?.name)
        .filter(Boolean)
        .join('，');
    }
  },

  onLoad(options) {
    const productId = options.id;
    this.loadProduct(productId);

    // #ifdef MP-WEIXIN
    wx.showShareMenu({
      withShareTicket: true,
      menus: ['shareAppMessage', 'shareTimeline']
    });
    // #endif
  },

  onShareAppMessage() {
    return {
      title: this.goods.title,
      path: `/pages/detail/detail?id=${this.goods.id}`
    };
  },
  onShareTimeline() {
    return {
      title: this.goods.title,
      query: `id=${this.goods.id}`
    };
  },

  methods: {
    /* -------------------- 商品详情 -------------------- */
    async loadProduct(id) {
      uni.showLoading({ title: '加载中' });
      try {
        const { data } = await uni.request({
          url: `/api/good/store/${id}/productDetails`
        });
        if (!data) throw new Error('商品不存在');

        this.goods = {
          id: data.id,
          title: data.name,
          price: data.price,
          stockQuantity: data.stockQuantity,
          img: data.imageUrl
        };

        this.slides = data.slideImages?.length
          ? data.slideImages
          : [data.imageUrl];

        this.specs = (data.specs || []).map(s => ({
          name: s.specName,
          values: (s.values || []).map(v => ({
            name: v.valueName,
            valueId: v.valueId,
            selected: false
          }))
        }));

        this.skus = (data.skus || []).map(s => ({
          skuId: s.skuId,
          price: s.price,
          stockQuantity: s.stockQuantity,
          skusSpecValues: (s.skusSpecValues || []).map(v => ({
            specId: v.specId,
            valueId: v.valueId
          }))
        }));

        this.hasSpec = this.specs.length > 0;

        // 无规格时把默认 SKU 设为当前
        if (!this.hasSpec && this.skus.length) {
          this.currentSku = this.skus[0];
        }

        this.promos = data.promos || [];
        this.services = data.services || [];
        this.descImgs = data.descImgs || [];
        this.shareList = data.shareList || [];

        /* 如果只有一种规格值，默认选中 */
        this.$nextTick(() => {
          this.specs.forEach((g, i) => {
            if (g.values.length === 1) this.choose(i, 0);
          });
        });
      } catch (e) {
        uni.showToast({ title: '商品信息获取失败', icon: 'none' });
      } finally {
        uni.hideLoading();
      }
    },

    /* -------------------- 规格选择 -------------------- */
    openSpec(addCartFlag) {
      this.isAddCart = addCartFlag;
      if (this.hasSpec) {
        this.specShow = true;
      } else {
        addCartFlag ? this.confirmAddCart() : this.confirmBuy();
      }
    },

    choose(groupIdx, valIdx) {
      this.specs[groupIdx].values.forEach((v, idx) => {
        this.$set(v, 'selected', idx === valIdx);
      });

      const selectedIds = this.specs
        .map(g => g.values.find(v => v.selected)?.valueId)
        .filter(Boolean);

      this.currentSku =
        this.skus.find(sku =>
          selectedIds.every(id =>
            sku.skusSpecValues.some(spec => spec.valueId === id)
          )
        ) || this.skus[0];

      if (this.currentSku) {
        this.goods.price = this.currentSku.price;
        this.goods.stockQuantity = this.currentSku.stockQuantity;
      }
    },

    /* -------------------- 加入购物车 -------------------- */
    confirmAddCart() {
      const skuId = this.hasSpec
        ? this.currentSku?.skuId
        : this.skus[0]?.skuId;
      const quantity = this.quantity;

      if (!skuId) {
        uni.showToast({ title: '商品信息异常', icon: 'none' });
        return;
      }

      uni.showLoading({ title: '加入中' });
      // TODO: 调接口
      uni.hideLoading();
      uni.showToast({ title: '已加入购物车', icon: 'success' });
      this.specShow = false;
	   uni.switchTab({
	    url: `/pages/personal/cart?skuId=${skuId}&num=${quantity}`
	  });
    },

    /* -------------------- 立即购买 -------------------- */
    confirmBuy() {
      // 1. 组装参数
       const skuId = this.hasSpec
         ? this.currentSku?.skuId
         : this.skus[0]?.skuId;
       const sku = this.hasSpec
         ? this.currentSku
         : this.skus[0];
     
       if (!sku) {
         uni.showToast({ title: 'SKU 异常', icon: 'none' });
         return;
       }
     
       // 2. 构造结算所需的商品数组（单件）
       const orderGoods = [{
         id: this.goods.id,
         skuId: sku.skuId,
         name: this.goods.title,
         cover: this.goods.img,
         price: sku.price,
         count: this.quantity,
         specText: this.selectedSpec || '默认规格'
       }];
     
       // 3. 存到全局缓存
       uni.setStorageSync('SETTLE_GOODS', orderGoods);
     
       // 4. 跳结算页
       uni.navigateTo({ url: '/pages/order/settle' });
    },

    /* -------------------- 其他 -------------------- */
    preview(index) {
      uni.previewImage({ current: index, urls: this.slides });
    },
    toHome() {
      uni.switchTab({ url: '/pages/index/indexTest' });
    },
    toCart() {
      uni.switchTab({ url: '/pages/personal/cart' });
    },
    toStore() {
       uni.navigateTo({ url: '/pages/store/storedetailTest' });
    },
    shareTo(platform) {
      uni.showToast({ title: '分享到' + platform, icon: 'none' });
    }
  }
};
</script>

<style lang="scss" scoped>
/* 极光白紫主题 2025-09-05 */
page {
  background: radial-gradient(circle at top, #F3E9FF 0%, #FFFFFF 100%);
  color: #240046;
}

/* 顶部轮播保持通透 */
.banner {
  width: 100%;
  height: 700rpx;
  border-radius: 0 0 32rpx 32rpx;
  overflow: hidden;
  box-shadow: 0 8rpx 24rpx rgba(157,78,221,0.10);
}

/* 商品信息卡片 */
.info {
  margin: 0 24rpx 24rpx;
  padding: 32rpx;
  background: #FFFFFF;
  border-radius: 24rpx;
  box-shadow: 0 4rpx 20rpx rgba(157,78,221,0.08);

  .title {
    font-size: 34rpx;
    font-weight: 600;
    color: #240046;
    line-height: 1.4;
    margin-bottom: 12rpx;
  }
  .price {
    display: flex;
    align-items: baseline;
    .now {
      font-size: 40rpx;
      font-weight: 700;
      background: linear-gradient(90deg, #9D4EDD 0%, #E0AAFF 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
    .old {
      margin-left: 12rpx;
      font-size: 26rpx;
      color: #9E9CB0;
      text-decoration: line-through;
    }
  }
}

/* 分享条 */
.share {
  margin: 0 24rpx 24rpx;
  padding: 20rpx 32rpx;
  background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
  border-radius: 16rpx;
  display: flex;
  align-items: center;
  color: #FFFFFF;
  font-size: 28rpx;
  box-shadow: 0 6rpx 18rpx rgba(157,78,221,0.20);
}

/* 通用卡片 */
.card {
  margin: 0 24rpx 24rpx;
  padding: 32rpx;
  background: #FFFFFF;
  border-radius: 24rpx;
  box-shadow: 0 4rpx 20rpx rgba(157,78,221,0.06);
}

/* 促销/服务 */
.promo-service .tag-item {
  display: flex;
  align-items: center;
  margin-bottom: 12rpx;
  .u-tag {
    border-color: #E0AAFF !important;
    color: #9D4EDD !important;
    background: #F9F5FF;
  }
}

/* 用户评价 */
.comment {
  display: flex;
  padding: 24rpx 0;
  border-bottom: 1rpx solid #F3E9FF;
  &:last-child { border: none; }
  .avatar {
    width: 72rpx;
    height: 72rpx;
    border-radius: 50%;
    border: 2rpx solid #E0AAFF;
    margin-right: 20rpx;
  }
  .body .name {
    font-size: 28rpx;
    font-weight: 600;
    color: #240046;
  }
  .cnt {
    margin: 8rpx 0;
    font-size: 28rpx;
    color: #3C3454;
    line-height: 1.5;
  }
  .time {
    font-size: 24rpx;
    color: #9E9CB0;
  }
}

/* 底部操作栏 */
.bottom-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  display: flex;
  align-items: center;
  padding: 14rpx 32rpx;
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(12rpx);
  border-top: 1rpx solid #F3E9FF;
  z-index: 100;

  .action {
    display: flex;
    flex-direction: column;
    align-items: center;
    font-size: 22rpx;
    color: #6F6C7B;
    margin-right: 32rpx;
  }

  .btns {
    margin-left: auto;
    display: flex;
    gap: 16rpx;
    .u-button {
      height: 80rpx;
      width: 216rpx;
      border-radius: 40rpx;
      font-size: 30rpx;
      font-weight: 600;
      box-shadow: 0 6rpx 18rpx rgba(157,78,221,0.18);
    }
    /* 加入购物车 */
    .u-button[type=warning] {
      background: linear-gradient(90deg, #E0AAFF 0%, #C77DFF 100%);
      border: none;
      color: #FFFFFF;
    }
    /* 立即购买 */
    .u-button[type=error] {
      background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
      border: none;
      color: #FFFFFF;
    }
    /* 按压效果 */
    .u-button:active {
      transform: scale(0.96);
      box-shadow: 0 2rpx 8rpx rgba(114,9,183,0.25);
    }
  }
}

/* 规格弹窗 */
.spec-panel {
  padding: 32rpx;
  background: #FFFFFF;
  border-radius: 32rpx 32rpx 0 0;
  .spec-top {
    display: flex;
    padding-bottom: 24rpx;
    margin-bottom: 24rpx;
    border-bottom: 1rpx solid #F3E9FF;
    image {
      width: 160rpx;
      height: 160rpx;
      border-radius: 16rpx;
      box-shadow: 0 4rpx 16rpx rgba(157,78,221,0.10);
    }
    .spec-price {
      font-size: 40rpx;
      font-weight: 700;
      background: linear-gradient(90deg, #9D4EDD 0%, #E0AAFF 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
    .spec-stock, .spec-sel {
      font-size: 26rpx;
      color: #6F6C7B;
      margin-top: 6rpx;
    }
    .spec-sel {
      color: #9D4EDD;
    }
  }

  .spec-title {
    font-size: 28rpx;
    font-weight: 600;
    color: #240046;
    margin: 24rpx 0 12rpx;
  }

  .spec-list {
    display: flex;
    flex-wrap: wrap;
    .spec-item {
      padding: 12rpx 32rpx;
      margin: 0 16rpx 16rpx 0;
      border: 2rpx solid #E0AAFF;
      border-radius: 28rpx;
      font-size: 26rpx;
      color: #6F6C7B;
      background: #F9F5FF;
      transition: all 0.2s;
      &.spec-item--active {
        border-color: #9D4EDD;
        color: #FFFFFF;
        background: linear-gradient(90deg, #9D4EDD 0%, #C77DFF 100%);
      }
    }
  }

  .spec-bottom {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 32rpx 0 24rpx;
    padding-top: 24rpx;
    border-top: 1rpx solid #F3E9FF;
  }

  .confirm-btn {
    width: 100%;
    height: 88rpx;
    border-radius: 44rpx;
    font-size: 32rpx;
    font-weight: 600;
    background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
    color: #FFFFFF;
    box-shadow: 0 6rpx 18rpx rgba(157,78,221,0.25);
  }
}

/* 分享弹窗 */
.share-panel {
  padding: 32rpx;
  background: #FFFFFF;
  border-radius: 32rpx 32rpx 0 0;
  .share-title {
    text-align: center;
    font-size: 32rpx;
    font-weight: 600;
    color: #240046;
    margin-bottom: 32rpx;
  }
  .cancel-btn {
    width: 100%;
    height: 80rpx;
    border-radius: 40rpx;
    font-size: 30rpx;
    color: #6F6C7B;
    background: #F9F5FF;
    border: 2rpx solid #E0AAFF;
  }
}
</style>