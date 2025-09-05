<template>
  <view class="settle-container">
    <!-- 返回按钮（如需要可放开） -->
    <!-- <view class="settle-header">
      <u-icon name="arrow-left" size="40rpx" color="#333" class="back-icon" @click="handleBack"/>
      <u-text text="确认订单" size="34rpx" color="#1a1a1a" fontWeight="600" class="title"/>
      <view class="empty-box"></view>
    </view> -->

    <!-- 配送方式 -->
    <view class="delivery-method">
      <view
        class="method-item"
        :class="{ active: deliveryMethod === 'delivery' }"
        @click="deliveryMethod = 'delivery'"
        hover-class="method-hover"
      >
        <u-icon name="car" size="40rpx" :color="deliveryMethod === 'delivery' ? '#ff7d00' : '#999'" />
        <u-text text="同城配送" align="center" size="28rpx" :color="deliveryMethod === 'delivery' ? '#ff7d00' : '#606266'" />
        <u-icon
          v-if="deliveryMethod === 'delivery'"
          name="checkmark-circle-fill"
          size="24rpx"
          color="#ff7d00"
        />
      </view>
      <view
        class="method-item"
        :class="{ active: deliveryMethod === 'pickup' }"
        @click="deliveryMethod = 'pickup'"
        hover-class="method-hover"
      >
        <u-icon name="bag" size="40rpx" :color="deliveryMethod === 'pickup' ? '#ff7d00' : '#999'" />
        <u-text text="门店自取" align="center" size="28rpx" :color="deliveryMethod === 'pickup' ? '#ff7d00' : '#606266'" />
        <u-icon
          v-if="deliveryMethod === 'pickup'"
          name="checkmark-circle-fill"
          size="24rpx"
          color="#ff7d00"
        />
      </view>
    </view>

    <!-- 收货地址 -->
    <view v-if="deliveryMethod === 'delivery'" class="card address-card card-hover" @click="chooseAddress">
      <u-icon name="map-fill" color="#ff7d00" size="44rpx" class="address-icon" />
      <view class="address-content">
        <view v-if="address && address.name" class="address-info">
          <view class="address-top">
            <u-text :text="address.name" size="30rpx" color="#1a1a1a" fontWeight="600" />
            <u-text :text="formatPhone(address.phone)" size="28rpx" color="#606266" />
          </view>
          <u-text
            :text="address.province + address.city + address.county + address.detail"
            size="26rpx"
            color="#606266"
            customStyle="line-height:1.5;word-break:break-all;margin-top:8rpx"
          />
        </view>
        <view v-else class="address-empty">
          <u-text text="请选择收货地址" size="28rpx" color="#909399" />
          <u-text text="点击添加或选择已有地址" size="24rpx" color="#c0c4cc" class="empty-tip" />
        </view>
      </view>
      <u-icon name="arrow-right" color="#ccc" size="28rpx" class="address-arrow" />
    </view>

    <!-- 自取门店 -->
    <view v-if="deliveryMethod === 'pickup'" class="card pickup-card">
      <view class="pickup-section">
        <u-text class="section-title" text="自提门店" size="28rpx" color="#1a1a1a" fontWeight="500" />
        <view class="store-info">
          <u-image
            width="100rpx"
            height="100rpx"
            radius="16rpx"
            :src="storeInfo.logo || '/static/default-store.png'"
            mode="aspectFill"
          />
          <view class="store-detail">
            <u-text
              :text="storeInfo.name || '未选择门店'"
              size="30rpx"
              color="#1a1a1a"
              fontWeight="600"
            />
            <view class="store-meta">
              <u-icon name="map-fill" size="22rpx" color="#909399" />
              <u-text :text="storeInfo.address || '暂无地址信息'" size="24rpx" color="#909399" />
            </view>
            <view class="store-meta">
              <u-icon name="clock-fill" size="22rpx" color="#909399" />
              <u-text :text="storeInfo.businessHours || '暂无营业时间'" size="24rpx" color="#909399" />
            </view>
          </view>
        </view>
      </view>

      <view class="pickup-section pickup-time-section">
        <u-text class="section-title" text="自提时间" size="28rpx" color="#1a1a1a" fontWeight="500" />
        <picker
          mode="date"
          :value="pickupTime"
          :start="todayDate"
          @change="onTimeChange"
        >
          <view class="time-picker picker-hover">
            <u-text
              :text="pickupTime ? formatDate(pickupTime) : '请选择自提日期'"
              size="28rpx"
              :color="pickupTime ? '#606266' : '#c0c4cc'"
            />
            <u-icon name="arrow-right" size="24rpx" color="#ccc" />
          </view>
        </picker>
      </view>
    </view>

    <!-- 商品清单 -->
    <view class="card goods-card">
      <view class="list-header">
        <u-text text="购物清单" size="32rpx" color="#1a1a1a" fontWeight="600" />
        <u-text :text="totalCount + '件商品'" size="26rpx" color="#909399" />
      </view>
      <view class="goods-list">
        <view
          v-for="(item, i) in validCartList"
          :key="i"
          class="goods-item"
        >
          <u-image
            width="120rpx"
            height="120rpx"
            radius="16rpx"
            :src="item.cover || '/static/default-goods.png'"
            mode="aspectFill"
          />
          <view class="goods-info">
            <u-text
              :text="item.name || '未命名商品'"
              size="30rpx"
              color="#1a1a1a"
              fontWeight="500"
              customStyle="line-height:1.4;-webkit-line-clamp:2;display:-webkit-box;overflow:hidden"
            />
            <u-text :text="item.specText || '默认规格'" size="24rpx" color="#909399" customStyle="margin-top:8rpx" />
            <view class="goods-price-count">
              <u-text :text="'¥' + (item.price || 0).toFixed(2)" size="30rpx" color="var(--primary-color)" fontWeight="600" />
              <u-text :text="'x' + item.count" size="26rpx" color="#909399" />
            </view>
          </view>
        </view>
        <view v-if="validCartList.length === 0" class="goods-empty">
          <u-icon name="shopping-cart-o" size="60rpx" color="#c0c4cc" />
          <u-text text="购物车为空" size="28rpx" color="#909399" />
          <u-text text="请先选择商品" size="24rpx" color="#c0c4cc" />
        </view>
      </view>
    </view>

    <!-- 费用明细 -->
    <view class="card fee-card">
      <view class="list-header">
        <u-text text="费用明细" size="32rpx" color="#1a1a1a" fontWeight="600" />
      </view>
      <view class="fee-list">
        <view class="fee-item">
          <u-text text="商品总价" size="28rpx" color="#606266" />
          <u-text :text="'¥' + goodsTotal.toFixed(2)" size="28rpx" color="#1a1a1a" />
        </view>
        <view v-if="deliveryMethod === 'delivery'" class="fee-item">
          <u-text text="配送费" size="28rpx" color="#606266" />
          <u-text
            :text="goodsTotal >= 200 ? '¥0.00 (满减)' : '¥' + deliveryCost.toFixed(2)"
            size="28rpx"
            :color="goodsTotal >= 200 ? '#00b42a' : '#1a1a1a'"
          />
        </view>
        <view v-if="deliveryMethod === 'pickup'" class="fee-item">
          <u-text text="自取优惠" size="28rpx" color="#606266" />
          <u-text text="-¥5.00" size="28rpx" color="#ff7d00" />
        </view>
        <view class="fee-item">
          <u-text text="优惠券抵扣" size="28rpx" color="#606266" />
          <u-text :text="'-¥' + (discount || 0).toFixed(2)" size="28rpx" color="#ff7d00" />
        </view>
        <view class="fee-item total-fee">
          <u-text text="实付款" size="32rpx" color="#1a1a1a" fontWeight="600" />
          <u-text
            :text="'¥' + finalAmount.toFixed(2)"
            size="36rpx"
            color="var(--primary-color)"
            fontWeight="700"
          />
        </view>
      </view>
    </view>

    <!-- 底部提交栏 -->
    <view class="submit-bar">
      <view class="total-amount">
        <u-text text="实付：" size="28rpx" color="#606266" />
        <u-text :text="'¥' + finalAmount.toFixed(2)" size="40rpx" color="var(--primary-color)" fontWeight="700" />
      </view>
      <u-button
        :text="deliveryMethod === 'delivery' ? '提交订单' : '确认自提'"
        type="primary"
        :disabled="!isSubmitEnable"
        :loading="isSubmitting"
        class="submit-btn"
        @click="submitOrder"
      />
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      storeInfo: {
        id: 0,
        name: '',
        address: '',
        phone: '暂无电话',
        businessHours: '暂无营业时间',
        notice: '暂无公告',
        logo: ''
      },
      deliveryMethod: 'delivery',          // delivery / pickup
      pickupTime: '',                      // 自取日期
      address: null,                       // 收货地址对象
      cartList: [],
      freight: 10,
      discount: 0,
      pickupDiscount: 5,
      isSubmitting: false,
      todayDate: ''
    };
  },
  onLoad() {
    const today = new Date();
    this.todayDate = today.toISOString().split('T')[0];
    this.pickupTime = this.todayDate;

    // 读取缓存的商品与门店
    this.cartList = uni.getStorageSync('SETTLE_GOODS') || [];
    this.storeInfo = uni.getStorageSync('CURRENT_STORE') || {};
  },
  onShow() {
    this.loadDefaultAddress();
    const selected = uni.getStorageSync('SELECTED_ADDRESS');
    if (selected && selected.name) {
      this.address = selected;
      uni.removeStorageSync('SELECTED_ADDRESS');
    }
  },
  computed: {
    validCartList() {
      return this.cartList.filter(i => i.name && i.price > 0);
    },
    totalCount() {
      return this.validCartList.reduce((s, i) => s + (i.count || 0), 0);
    },
    goodsTotal() {
      return this.validCartList.reduce((s, i) => s + i.price * i.count, 0);
    },
    deliveryCost() {
      return this.deliveryMethod === 'pickup' ? 0 : (this.goodsTotal >= 200 ? 0 : this.freight);
    },
    finalAmount() {
      let amt = this.goodsTotal + this.deliveryCost - this.discount;
      if (this.deliveryMethod === 'pickup') amt -= this.pickupDiscount;
      return Math.max(parseFloat(amt.toFixed(2)), 0);
    },
    isSubmitEnable() {
      if (!this.validCartList.length) return false;
      return this.deliveryMethod === 'delivery'
        ? !!(this.address && this.address.name)
        : !!this.pickupTime;
    }
  },
  methods: {
    loadDefaultAddress() {
      const uid = uni.getStorageSync('userInfo')?.id;
      if (!uid) return;
      uni.request({
        url: '/api/User/defaultaddressinfo',
        method: 'GET',
        data: { userid: uid },
        success: res => {
          if (res.data && res.data.name) this.address = res.data;
        }
      });
    },
    chooseAddress() {
      uni.navigateTo({ url: '/pages/personal/address?from=detail' });
    },
    onTimeChange(e) {
      this.pickupTime = e.detail.value;
    },
    formatPhone(phone) {
      return phone ? phone.replace(/(\d{3})\d{4}(\d{4})/, '$1****$2') : '暂无电话';
    },
    formatDate(str) {
      if (!str) return '';
      const [y, m, d] = str.split('-');
      return `${y}年${m.padStart(2, '0')}月${d.padStart(2, '0')}日`;
    },
    submitOrder() {
		console.log('准备提交的 Items:', this.validCartList);
		if (!this.validCartList || this.validCartList.length === 0) {
		  uni.showToast({ title: '请先选择商品', icon: 'none' });
		  return;
		}
      if (!this.isSubmitEnable) return;
      this.isSubmitting = true;
      uni.showLoading({ title: '提交中...', mask: true });

      const user = uni.getStorageSync('userInfo');
      if (!user?.id) {
        uni.showToast({ title: '请先登录', icon: 'none' });
        this.isSubmitting = false;
        uni.hideLoading();
        return;
      }

      const payload = {
		  
         CustomerId: user.id,
         StoreId: parseInt(this.storeInfo.id) || 0,
         DeliveryType: this.deliveryMethod === 'delivery' ? 1 : 0,
         PickupDate: this.deliveryMethod === 'pickup' ? this.pickupTime : null,
         Address: this.deliveryMethod === 'delivery' ? {
           Name: this.address.name,
           Phone: this.address.phone,
           Province: this.address.province,
           City: this.address.city,
           County: this.address.county,
           Detail: this.address.detail
         } : null,
         Items: this.validCartList.map(i => ({
           SkuId:    Number(i.id),
             Quantity: Number(i.count),
             Price:    Number(i.price)
         }))
       };
	   console.log(payload);
      uni.request({
        url: '/api/order/submitorder',
        method: 'POST',
        data:  payload,
        header: { 'Content-Type': 'application/json' },
        success: res => {
          uni.hideLoading();
          if (res.statusCode === 200) {
            uni.showToast({ title: '订单提交成功', icon: 'success' });
            console.log('后端返回的数据:', res.data); // 打印后端返回的数据
                uni.setStorageSync('ORDER_RESULT', {
                  orderNo: res.data.code, // 注意字段大小写是否与后端返回一致
                  amount: res.data.totalAmount,
                  deliveryMethod: res.data.deliveryType === 1 ? 'delivery' : 'pickup',
                  address: res.data.deliveryType === 1
                    ? {
                        name: JSON.parse(res.data.shippingAddress).Name,
                        phone: JSON.parse(res.data.shippingAddress).Phone,
                        province: JSON.parse(res.data.shippingAddress).Province,
                        city: JSON.parse(res.data.shippingAddress).City,
                        county: JSON.parse(res.data.shippingAddress).County,
                        detail: JSON.parse(res.data.shippingAddress).Detail
                      }
                    : null,
                  store: res.data.deliveryType === 0
                    ? {id:this.storeInfo.id, name: this.storeInfo.name, address: this.storeInfo.address }
                    : {id:this.storeInfo.id},
                  pickupTime: res.data.pickUpDate,
                  goods: payload.Items.map(i => {
                    const g = this.validCartList.find(v => v.id == i.SkuId);
                    return {
                      id: i.SkuId,
                      name: g?.name || '',
                      cover: g?.cover || '/static/default-goods.png',
                      spec: g?.specText || '',
                      price: i.Price,
                      count: i.Quantity
                    };
                  })
                });
                setTimeout(() => uni.navigateTo({ url: '/pages/order/result?status=pending' }), 1500);
          } else {
            const msg = typeof res.data === 'string' ? res.data : JSON.stringify(res.data || '提交失败');
            uni.showToast({ title: msg, icon: 'none', duration: 3000 });
            console.log(msg);
          }
        },
        fail: () => uni.showToast({ title: '网络错误', icon: 'none' }),
        complete: () => {
          this.isSubmitting = false;
          uni.hideLoading();
        }
      });
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

/* 顶部导航 */
.settle-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24rpx 32rpx;
  background: #FFFFFF;
  border-bottom: 1rpx solid #F3E9FF;
  .back-icon {
    color: #6F6C7B !important;
  }
  .title {
    flex: 1;
    text-align: center;
    font-weight: 600;
    font-size: 36rpx;
    color: #240046;
  }
  .empty-box {
    width: 40rpx;
  }
}

/* 配送方式 */
.delivery-method {
  margin: 24rpx;
  background: #FFFFFF;
  border-radius: 24rpx;
  box-shadow: 0 4rpx 20rpx rgba(157, 78, 221, 0.06);
  overflow: hidden;
  .method-item {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 32rpx 0;
    gap: 12rpx;
    position: relative;
    transition: all 0.3s;
    &.active {
      background: rgba(157, 78, 221, 0.06);
      color: #9D4EDD;
    }
    &:active {
      background: rgba(157, 78, 221, 0.1);
    }
  }
}

/* 地址 / 自取卡片 */
.card {
  margin: 0 24rpx 24rpx;
  padding: 32rpx;
  background: #FFFFFF;
  border-radius: 24rpx;
  box-shadow: 0 4rpx 20rpx rgba(157, 78, 221, 0.06);
  &.address-card {
    display: flex;
    align-items: flex-start;
    gap: 20rpx;
    .address-icon {
      color: #9D4EDD !important;
    }
    .address-arrow {
      color: #C77DFF !important;
    }
  }
  &.pickup-card {
    .store-info {
      display: flex;
      gap: 20rpx;
      .store-logo {
        border: 1rpx solid #F3E9FF;
        border-radius: 16rpx;
      }
      .store-meta {
        display: flex;
        align-items: center;
        gap: 8rpx;
        margin-top: 8rpx;
        color: #6F6C7B;
      }
    }
    .time-picker {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20rpx;
      background: #F9F5FF;
      border-radius: 16rpx;
      margin-top: 20rpx;
    }
  }
}

/* 商品清单 */
.goods-card {
  .list-header {
    display: flex;
    justify-content: space-between;
    margin-bottom: 24rpx;
    padding-bottom: 16rpx;
    border-bottom: 1rpx solid #F3E9FF;
  }
  .goods-item {
    display: flex;
    gap: 20rpx;
    margin-bottom: 24rpx;
    .goods-img {
      border-radius: 16rpx;
      border: 1rpx solid #F3E9FF;
    }
    .goods-info {
      flex: 1;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
    }
    .goods-price-count {
      display: flex;
      justify-content: space-between;
      margin-top: 12rpx;
      .price {
        font-weight: 600;
        background: linear-gradient(90deg, #9D4EDD 0%, #E0AAFF 100%);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
      }
    }
  }
  .goods-empty {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 60rpx 0;
    gap: 16rpx;
    color: #6F6C7B;
  }
}

/* 费用明细 */
.fee-card {
  .list-header {
    margin-bottom: 24rpx;
    padding-bottom: 16rpx;
    border-bottom: 1rpx solid #F3E9FF;
  }
  .fee-item {
    display: flex;
    justify-content: space-between;
    padding: 18rpx 0;
    border-bottom: 1rpx solid #F3E9FF;
    &:last-child {
      border: none;
      padding-top: 24rpx;
    }
    &.total-fee {
      .fee-name {
        font-size: 32rpx;
        font-weight: 600;
        color: #240046;
      }
      .fee-value {
        font-size: 36rpx;
        font-weight: 700;
        background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
      }
    }
  }
}

/* 底部提交栏 */
.submit-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24rpx 32rpx;
  background: rgba(255, 255, 255, 0.92);
  backdrop-filter: blur(12rpx);
  border-top: 1rpx solid #F3E9FF;
  z-index: 99;
  .total-amount {
    display: flex;
    align-items: baseline;
    gap: 8rpx;
    .amount {
      font-size: 40rpx;
      font-weight: 700;
      background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
  }
  .submit-btn {
    width: 260rpx;
    height: 88rpx;
    border-radius: 44rpx;
    font-size: 32rpx;
    font-weight: 600;
    background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
    border: none;
    color: #FFFFFF;
    box-shadow: 0 6rpx 18rpx rgba(157, 78, 221, 0.25);
    &:active {
      transform: scale(0.96);
    }
    &:disabled {
      background: #C0C4CC !important;
      color: #FFFFFF !important;
    }
  }
}
</style>