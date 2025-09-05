<template>
  <view class="result-container">
    <!-- 1. 状态图标 + 标题 -->
    <view class="status-bar">
      <u-icon :name="iconMap[status]" :color="colorMap[status]" size="120rpx"  :label="titleMap[status]"></u-icon>
      <view class="status-title">{{  }}</view>
    </view>

    <!-- 2. 订单号 + 金额 -->
    <view class="card order-summary">
      <view>订单号：{{ orderNo }}</view>
      <view>实付金额：<text class="price">¥{{ amount }}</text></view>
    </view>

    <!-- 3. 收货 / 自提信息 -->
    <view class="card info-card">
      <block v-if="deliveryMethod === 'delivery'">
        <view>收货人：{{ address.name }} {{ address.phone }}</view>
        <view>{{ address.province }}{{ address.city }}{{ address.county }}{{ address.detail }}</view>
      </block>
      <block v-else>
        <view>自提门店：{{ store.name }}</view>
        <view>{{ store.address }}</view>
        <view>自提日期：{{ pickupTime }}</view>
      </block>
    </view>

    <!-- 4. 商品清单 -->
    <view class="card goods-card">
      <view class="title">商品清单</view>
      <view v-for="item in goods" :key="item.id" class="goods-row">
        <u-image :src="item.cover" width="100rpx" height="100rpx" radius="8rpx"></u-image>
        <view class="goods-info">
          <view class="name">{{ item.name }}</view>
          <view class="spec">{{ item.spec }}</view>
          <view class="price-count">
            ¥{{ item.price }} × {{ item.count }}
          </view>
        </view>
      </view>
    </view>

    <!-- 5. 倒计时（待支付时） -->
    <view v-if="status === 'pending'" class="card countdown">
      请在
      <u-count-down :time="countdown" format="mm:ss" @finish="status = 'fail'"></u-count-down>
      内完成支付
    </view>

    <!-- 6. 操作按钮 -->
    <view class="action-group">
      <u-button v-if="status === 'pending'" type="primary" text="立即支付" @click="goPay"></u-button>
      <u-button v-if="status === 'fail'" type="primary" text="重新支付" @click="goPay"></u-button>
      <u-button plain type="default" text="返回首页" @click="goHome"></u-button>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      status: 'pending',
      orderNo: '',
      amount: 0,
      deliveryMethod: 'delivery',
      address: {},
      store: {},
      pickupTime: '',
      goods: [],
      countdown: 15 * 60 * 1000
    };
  },
  onLoad(opt) {
    const data = uni.getStorageSync('ORDER_RESULT') || {}; // 从缓存中读取订单结果数据，如果不存在则使用空对象
	 console.log(data);
      this.status = opt.status || (data.deliveryMethod ? 'pending' : 'success'); // 根据 URL 参数或订单的配送方式设置初始状态
      this.orderNo = data.orderNo || data.Code || 'SN0000'; // 从缓存中读取订单号，如果不存在则使用默认值 'SN0000'
      this.amount = data.amount || data.TotalAmount || 0; // 从缓存中读取订单金额，如果不存在则默认为 0
      this.deliveryMethod = data.deliveryMethod || 'delivery'; // 从缓存中读取配送方式，如果不存在则默认为 'delivery'
      this.address = data.address || {}; // 从缓存中读取收货地址信息，如果不存在则使用空对象
      this.store = data.store || {}; // 从缓存中读取自提门店信息，如果不存在则使用空对象
      this.pickupTime = data.pickupTime || ''; // 从缓存中读取自提时间，如果不存在则使用空字符串
      this.goods = data.goods || []; 
	 
  },
  computed: {
    iconMap() {
      return { success: 'checkmark-circle-fill', pending: 'clock-fill', fail: 'close-circle-fill' };
    },
    colorMap() {
      return { success: '#00b42a', pending: '#00aaff', fail: '#ff0000' };
    },
    titleMap() {
      return { success: '支付成功', pending: '等待支付', fail: '支付失败' };
    }
  },
  methods: {
    async goPay() {
      uni.showLoading({ title: '正在前往支付…', mask: true });
			uni.request({
			   url: `/api/order/pay?orderNo=${this.orderNo}`,
			    method: 'POST',
			    data: {},
			  responseType: 'text', // 必须
			  success: (res) => {
			    if (res.statusCode === 200) {
			     const div = document.createElement('div');
			     div.innerHTML = res.data;
			     document.body.appendChild(div);
			     div.querySelector('form').submit();
			    } else {
			      uni.showToast({ title: '支付请求失败', icon: 'none' })
			    }
			  }
			})
        },
    goHome() {
      uni.switchTab({ url: '/pages/index/index' });
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

.result-container {
  padding: 32rpx;
  min-height: 100vh;
}

/* 1. 状态栏 */
.status-bar {
  text-align: center;
  margin-bottom: 40rpx;
  .u-icon {
    filter: drop-shadow(0 4rpx 8rpx rgba(157,78,221,0.20));
  }
  .status-title {
    font-size: 40rpx;
    font-weight: 600;
    margin-top: 24rpx;
    letter-spacing: 2rpx;
  }
}

/* 2. 通用卡片 */
.card {
  background: #FFFFFF;
  border-radius: 24rpx;
  padding: 32rpx;
  margin-bottom: 24rpx;
  box-shadow: 0 4rpx 24rpx rgba(157,78,221,0.08);
}

/* 3. 订单摘要 */
.order-summary {
  display: flex;
  flex-direction: column;
  gap: 12rpx;
  font-size: 28rpx;
  color: #6F6C7B;
  .price {
    font-size: 36rpx;
    font-weight: 700;
    background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
  }
}

/* 4. 地址/自提信息 */
.info-card {
  display: flex;
  flex-direction: column;
  gap: 12rpx;
  font-size: 28rpx;
  line-height: 1.6;
  color: #240046;
}

/* 5. 商品清单 */
.goods-card {
  .title {
    font-size: 32rpx;
    font-weight: 600;
    margin-bottom: 24rpx;
    color: #240046;
  }
  .goods-row {
    display: flex;
    gap: 24rpx;
    margin-bottom: 24rpx;
    &:last-child { margin-bottom: 0; }
    .u-image {
      flex-shrink: 0;
      border: 1rpx solid #F3E9FF;
      border-radius: 16rpx;
    }
  }
  .goods-info {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    .name {
      font-size: 30rpx;
      color: #240046;
      font-weight: 500;
    }
    .spec {
      font-size: 26rpx;
      color: #6F6C7B;
      margin-top: 4rpx;
    }
    .price-count {
      margin-top: 12rpx;
      font-size: 28rpx;
      color: #9D4EDD;
      font-weight: 600;
    }
  }
}

/* 6. 倒计时 */
.countdown {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12rpx;
  font-size: 28rpx;
  color: #6F6C7B;
  .u-count-down {
    font-size: 32rpx;
    font-weight: 700;
    color: #7209B7;
  }
}

/* 7. 底部按钮组 */
.action-group {
  position: fixed;
  bottom: 40rpx;
  left: 32rpx;
  right: 32rpx;
  display: flex;
  flex-direction: column;
  gap: 20rpx;
  .u-button {
    height: 88rpx;
    border-radius: 44rpx;
    font-size: 30rpx;
    font-weight: 600;
    box-shadow: 0 6rpx 18rpx rgba(157,78,221,0.18);
    &:active {
      transform: scale(0.98);
    }
    /* 立即支付 */
    &.u-button--primary {
      background: linear-gradient(90deg, #9D4EDD 0%, #7209B7 100%);
      border: none;
      color: #FFFFFF;
    }
    /* 返回首页 */
    &.u-button--default {
      background: #F9F5FF;
      border: 2rpx solid #E0AAFF;
      color: #6F6C7B;
    }
  }
}
</style>