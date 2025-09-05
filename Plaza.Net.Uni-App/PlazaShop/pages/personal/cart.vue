<template>
  <view class="page">
    <!-- 空购物车 -->
    <u-empty
      v-if="cartList.length === 0"
      mode="car"
      text="购物车空空如也"
      margin-top="200rpx"
    />

    <!-- 商品列表 -->
    <scroll-view
      v-else
      class="scroll-box"
      scroll-y
    >
      <view class="list-inner">
        <view
          v-for="(shop, sIndex) in cartList"
          :key="shop.sto_id"
          class="shop-wrap"
        >
          <!-- 吸顶店铺标题（u-sticky 官方控件） -->

            <view class="shop-header">
              <u-checkbox-group
                :value="shop.selectedIds"
                @change="shopChange(sIndex)"
              >
                <u-checkbox
                  :name="shop.sto_name"
                  shape="circle"
                  active-color="#a855f7"
                />
              </u-checkbox-group>

              <!-- 店铺名（弹性区域，超长省略） -->
              <u-text
                :text="shop.sto_name"
                size="28rpx"
                margin="0 0 0 12rpx"
                lines="1"
                style="flex: 1;"
              />

              <!-- 固定宽度操作区（不位移） -->
              <view class="shop-extra">
                <!-- 领券图标（占位，不显示也占宽度） -->
                <view v-if="false" class="icon-btn">
                  <u-icon name="coupon" color="#94a3b8" size="20"/>
                </view>

                <!-- 进店图标 -->
                <view class="icon-btn" @click="goStore(shop.sto_id)">
                  <u-icon name="arrow-right" color="#a855f7" size="20"/>
                  <u-text text="进店" size="24rpx" color="#a855f7" margin="0 0 0 4rpx"/>
                </view>
              </view>
            </view>
  

          <!-- 商品（u-swipe-action 保留） -->
          <u-swipe-action>
            <u-swipe-action-item
              v-for="(goods, gIndex) in shop.goods"
              :key="goods.id"
              :options="swipeOptions"
              @click="handleSwipe($event, sIndex, gIndex)"
            >
              <view class="goods-box">
                <!-- 复选 -->
                <u-checkbox-group
                  :value="selectedGoods"
                  @change="goodsChange"
                >
                  <u-checkbox
                    :name="goods.id"
                    shape="circle"
                    active-color="#a855f7"
                  />
                </u-checkbox-group>

                <!-- 图片 -->
                <u-image
                  :src="goods.good_img"
                  width="140rpx"
                  height="140rpx"
                  radius="16"
                  margin="0 20rpx"
                />

                <!-- 信息 -->
                <view class="goods-info">
                  <u-text
                    :text="goods.good_name"
                    size="28rpx"
                    lines="2"
                  />
                  <u-text
                    text="7天无理由"
                    size="20rpx"
                    color="#94a3b8"
                    margin="6rpx 0 0 0"
                  />
                  <u-text
                    :text="'规格：' + goods.spec + ' | 库存：' + goods.stock"
                    size="22rpx"
                    color="#94a3b8"
                    margin="6rpx 0 0 0"
                  />
                  <view class="price-line">
                    <u-text
                      :text="'￥' + goods.good_price.toFixed(2)"
                      color="#a855f7"
                      size="30rpx"
                      bold
                    />
                    <u-number-box
                      v-model="goods.good_num"
                      :min="1"
                      :max="goods.stock"
                      integer
                      size="24rpx"
                      @change="calcTotal"
                      @blur="calcTotal"
                    />
                  </view>
                </view>
              </view>
            </u-swipe-action-item>
          </u-swipe-action>
        </view>
      </view>
    </scroll-view>

    <!-- 结算栏（极光白紫） -->
    <view class="footer">
      <u-checkbox-group :value="allIds" @change="allChange">
        <u-checkbox name="all" shape="circle" active-color="#a855f7">
          <u-text text="全选" size="26rpx" margin="0 0 0 8rpx" />
        </u-checkbox>
      </u-checkbox-group>

      <view class="total-box">
        <u-text text="合计：" size="26rpx" />
        <u-text
          :text="'￥' + total.toFixed(2)"
          color="#a855f7"
          size="30rpx"
          bold
        />
        <u-text
          v-if="discount"
          :text="'(已省￥' + discount.toFixed(2) + ')'"
          size="22rpx"
          color="#94a3b8"
        />
      </view>

      <u-button type="primary" shape="circle" size="small" @click="onSubmit">
        <u-text :text="'结算(' + sum + ')'" size="26rpx" color="#fff" />
      </u-button>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      plazaId: null,
      userId: null,
      swipeOptions: [
        { text: '移入收藏', style: { backgroundColor: '#0ea5e9' } },
        { text: '删除', style: { backgroundColor: '#f59e0b' } }
      ],
      cartList: [],
      selectedGoods: [],
      allIds: []
    };
  },
  computed: {
    total() {
      let t = 0;
      this.cartList.forEach(shop =>
        shop.goods.forEach(g => {
          if (this.selectedGoods.includes(g.id)) {
            t += g.good_price * g.good_num;
          }
        })
      );
      return t;
    },
    discount() {
      return this.total > 100 ? this.total * 0.05 : 0;
    },
    sum() {
      let s = 0;
      this.cartList.forEach(shop =>
        shop.goods.forEach(g => {
          if (this.selectedGoods.includes(g.id)) s += g.good_num;
        })
      );
      return s;
    }
  },
  onLoad(opt) {
    this.plazaId = Number(uni.getStorageSync('selectedPlaza')?.id || 0);
    this.userId = uni.getStorageSync('userInfo')?.id;
    this.loadCart();
  },
  methods: {
    goStore(storeId) {
      uni.navigateTo({ url: `/pages/store/storedetailTest?id=${storeId}` });
    },
    async loadCart() {
      uni.showLoading({ title: '加载中' });
      try {
        const res = await uni.request({
          url: `/api/Cart/shopcart`,
          method: 'GET',
          data: { userid: this.userId, plazaid: this.plazaId, pageIndex: 1, pageSize: 120 }
        });
        this.formatCart(res.data || []);
      } finally {
        uni.hideLoading();
      }
    },
    formatCart(list) {
      this.cartList = list.map(store => ({
        sto_id: store.storeId,
        sto_name: store.storeName,
        sto_logo: store.storeLogo,
        selectedIds: [],
        goods: store.items.map(item => ({
          id: item.skuId,
          cartId: item.cartId,
          good_name: item.name,
          good_img: item.imageUrl,
          good_price: Number(item.price),
          good_num: item.quantity,
          stock: item.stock,
          spec: item.specText,
          selected: item.selected
        }))
      }));

      this.selectedGoods = [];
      this.cartList.forEach(s => {
        s.goods.forEach(g => {
          if (g.selected) this.selectedGoods.push(g.id);
        });
      });
      this.syncAll();
    },
    shopChange(sIndex) {
      const shop = this.cartList[sIndex];
      const checked = !shop.selectedIds.includes(shop.sto_name);
      const ids = shop.goods.map(g => g.id);
      if (checked) {
        ids.forEach(id => {
          if (!this.selectedGoods.includes(id)) this.selectedGoods.push(id);
        });
        shop.selectedIds = [shop.sto_name];
      } else {
        ids.forEach(id => {
          const idx = this.selectedGoods.indexOf(id);
          if (idx > -1) this.selectedGoods.splice(idx, 1);
        });
        shop.selectedIds = [];
      }
      this.syncAll();
    },
    goodsChange(e) {
      this.selectedGoods = [...e.detail.value];
      this.syncShop();
      this.syncAll();
    },
    allChange(e) {
      const checked = e.detail.value.includes('all');
      this.selectedGoods = checked
        ? this.cartList.flatMap(shop => shop.goods.map(g => g.id))
        : [];
      this.cartList.forEach(shop => {
        shop.selectedIds = checked ? [shop.sto_name] : [];
      });
      this.allIds = checked ? ['all'] : [];
    },
    syncShop() {
      this.cartList.forEach(shop => {
        const ids = shop.goods.map(g => g.id);
        const selected = ids.filter(id => this.selectedGoods.includes(id));
        shop.selectedIds = selected.length === ids.length ? [shop.sto_name] : [];
      });
    },
    syncAll() {
      const allIds = this.cartList.flatMap(shop => shop.goods.map(g => g.id));
      this.allIds =
        allIds.length && allIds.every(id => this.selectedGoods.includes(id))
          ? ['all']
          : [];
    },
    calcTotal() {
      this.$nextTick(() => {
        this.syncShop();
        this.syncAll();
      });
    },
    handleSwipe(e, sIndex, gIndex) {
      if (e.index === 1) {
        const g = this.cartList[sIndex].goods[gIndex];
        uni.request({
          url: `/api/cart/delcart?id=${g.cartId}`,
          method: 'POST',
          data: { isDeleted: true }
        }).then(() => {
          const idx = this.selectedGoods.indexOf(g.id);
          if (idx > -1) this.selectedGoods.splice(idx, 1);
          this.cartList[sIndex].goods.splice(gIndex, 1);
          if (!this.cartList[sIndex].goods.length) this.cartList.splice(sIndex, 1);
          this.syncShop();
          this.syncAll();
        });
      } else {
        uni.$u.toast('已移入收藏');
      }
    },
    onSubmit() {
      if (!this.sum) {
        uni.$u.toast('请选择商品');
        return;
      }
      uni.navigateTo({
        url: `/pages/order/create?cart=1&total=${(this.total - this.discount).toFixed(2)}`
      });
    }
  }
};
</script>

<style lang="scss" scoped>
/* 极光白紫 - 零变量 */
page {
  background-color: #f8fafc;
}

.page {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f8fafc;
}

/* 顶部标题 */
.nav-top {
  background: #ffffff;
  padding: 44rpx 32rpx 24rpx;
  .nav-title {
    font-size: 40rpx;
    font-weight: 600;
    color: #1e293b;
  }
}

/* 列表区 */
.scroll-box {
  flex: 1;
  height: 0; /* 小程序兼容 */
  padding-bottom: 120rpx;
  box-sizing: border-box;
}

.list-inner {
  padding: 0 20rpx 40rpx;
}

/* 店铺卡片（紫银描边 + 微阴影） */
.shop-wrap {
  margin-bottom: 20rpx;
  background: #ffffff;
  border-radius: 24rpx;
  overflow: hidden;
  box-shadow: 0 4rpx 20rpx rgba(124, 58, 237, 0.08);
  border: 1rpx solid rgba(124, 58, 237, 0.12);
}

/* 吸顶店铺标题（不位移） */
.shop-header {
  display: flex;
  align-items: center;
  padding: 24rpx 32rpx;
  background: linear-gradient(135deg, rgba(124, 58, 237, 0.06) 0%, rgba(168, 85, 247, 0.04) 100%);
  border-bottom: 1rpx solid rgba(124, 58, 237, 0.08);
}
.shop-extra {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 16rpx;
  width: 120rpx; /* 固定宽度，吸顶不位移 */
  justify-content: flex-end;
}

.icon-btn {
  display: flex;
  align-items: center;
  padding: 8rpx 12rpx;
  border-radius: 16rpx;
  background: rgba(124, 58, 237, 0.08);
}

/* 商品卡片 */
.goods-box {
  display: flex;
  align-items: flex-start;
  padding: 24rpx 32rpx;
  background: #ffffff;
  &:not(:last-child) {
    border-bottom: 1rpx solid #f1f5f9;
  }
}
.goods-info {
  flex: 1;
  margin-left: 20rpx;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}
.price-line {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: 12rpx;
}

/* 底部结算栏（极光白紫） */
.footer {
  position: fixed;
  left: 0;
  right: 0;
  bottom: 0;
  height: 100rpx;
  background: #ffffff;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24rpx;
  box-shadow: 0 -4rpx 12rpx rgba(124, 58, 237, 0.08);
  border-top: 1rpx solid rgba(124, 58, 237, 0.08);

  /* #ifdef H5 */
  z-index: 999;
  padding-bottom: constant(safe-area-inset-bottom);
  padding-bottom: env(safe-area-inset-bottom);
  bottom: 50px;
  /* #endif */
}
.total-box {
  display: flex;
  align-items: center;
  gap: 8rpx;
}
</style>