<template>
  <view class="page">
    <!-- 吸顶分段器 - 极光紫风格 -->
    <u-sticky offset-top="0">
      <view class="tab-aurora">
        <u-subsection
          :list="tabList"
          :current="current"
          activeColor="#a855f7"
          inactiveColor="#64748b"
          bgColor="#f8fafc"
          bold
          @change="handleTabChange"
        />
      </view>
    </u-sticky>

    <!-- 订单列表 -->
    <swiper 
      class="swiper-box" 
      :current="current" 
      @change="handleSwiperChange"
      :style="{ height: swiperHeight + 'px' }"
    >
      <swiper-item v-for="(tabItem, tabIndex) in tabList" :key="tabIndex">
        <scroll-view
          scroll-y
          class="scroll-area"
          refresher-enabled
          refresher-background="#f8fafc"
          :refresher-triggered="refresherTrigger[tabIndex]"
          @refresherrefresh="handleRefresh"
          @scrolltolower="handleLoadMore"
        >


          <!-- 空态 - 极光风格 -->
          <view v-if="isLoading[tabIndex]" class="empty-box">
            <image src="/static/empty-order.png" mode="aspectFit" class="empty-img" />
            <text class="empty-txt">暂无订单</text>
            <view class="empty-btn" @click="handleGoShopping">去逛逛</view>
          </view>

          <!-- 订单卡片（层次优化） -->
          <view v-else>
            <view 
              v-for="(order, orderIndex) in orderList[tabIndex]" 
              :key="order.id" 
              class="order-card"
            >
              <!-- 渐变标题栏 -->
              <view class="card-header">
                <u-icon name="bag" color="#a855f7" size="32" />
                <text class="shop-name">{{ order.storeName }}</text>
                <text class="plaza-name">· {{ order.plazaName }}</text>
                <view class="status-tag" :class="getStatusClass(order.orderStatus)">
                  {{ order.orderStatus }}
                </view>
              </view>

              <!-- 商品 -->
              <view 
                v-for="(item, itemIndex) in order.items" 
                :key="`${order.id}-${item.skuId}`" 
                class="goods-box" 
                @click="handleOrderDetail(order.id)"
              >
                <u-image 
                  width="140rpx" 
                  height="140rpx" 
                  radius="16" 
                  :src="item.imageUrl || '/static/logo.png'" 
                  mode="aspectFill" 
                  lazy-load 
                />
                <view class="goods-info">
                  <text class="title u-line-2">{{ item.title }}</text>
                  <text class="spec">{{ item.spec || '默认规格' }}</text>
                  <view class="price-line">
                    <text class="price">¥{{ item.price }}</text>
                    <text class="num">x{{ item.quantity }}</text>
                  </view>
                </view>
              </view>

              <!-- 合计 -->
              <view class="total-line">
                <text>共{{ order.items.length }}件</text>
                <text class="total-txt">
                  合计：<text class="total-price">¥{{ order.totalAmount }}</text>
                </text>
              </view>

              <!-- 操作栏 - 修复事件绑定 -->
               <view class="action-box">
                 <view 
                   v-for="(btn, idx) in getOrderActions(order)" 
                   :key="idx" 
                   class="btn"
                   :class="btn.type" 
                   @click="btn.click"
                 >
                   {{ btn.text }}
                 </view>
               </view>
            </view>

            <u-loadmore 
              :status="loadStatus[tabIndex]" 
              margin-top="20" 
              color="#94a3b8" 
            />
          </view>
        </scroll-view>
      </swiper-item>
    </swiper>
  </view>
</template>

<script>
export default {
 
  data() {
    return {
      tabList: [],
      current: 0,
      orderList: [],
      loadStatus: [],
      refresherTrigger: [],
      pageMap: [],
      loadingLock: [],
      isLoading: [],
      emptyText: [],
      swiperHeight: 0,
      loadText: {
        loadmore: '上拉加载更多',
        loading: '努力加载中',
        nomore: '没有更多了'
      }
    };
  },

  async onLoad() {
    await this.initData();
    this.calculateSwiperHeight();
  },

  onShow() {
    // 页面显示时重新计算高度
    this.$nextTick(() => {
      this.calculateSwiperHeight();
    });
  },

  methods: {
    // 初始化数据
    async initData() {
      try {
        const res = await uni.$u.http.request({
          url: '/api/order/orderStatu',
          method: 'GET'
        });
        
        this.tabList = [
          { name: '全部', statusId: null }, 
          ...res.data.map(i => ({ name: i.name, statusId: i.id }))
        ];
        
        const len = this.tabList.length;
        this.orderList = new Array(len).fill([]).map(() => []);
        this.loadStatus = new Array(len).fill('loadmore');
        this.refresherTrigger = new Array(len).fill(false);
        this.pageMap = new Array(len).fill(1);
        this.loadingLock = new Array(len).fill(false);
        this.isLoading = new Array(len).fill(false);
        this.emptyText = new Array(len).fill('这里空空如也~');
        
        await this.loadData(0, true);
      } catch (error) {
        uni.showToast('初始化失败，请重试');
      }
    },

    // 计算swiper高度
    calculateSwiperHeight() {
      uni.createSelectorQuery()
        .in(this)
        .select('.page')
        .boundingClientRect(rect => {
          if (rect) {
            this.swiperHeight = rect.height - 88;
          }
        })
        .exec();
    },

    // 统一的错误处理
    handleError(error, tabIndex) {
      console.error('Order list error:', error);
      this.$set(this.loadStatus, tabIndex, 'loadmore');
      this.$set(this.isLoading, tabIndex, false);
      uni.showToast('加载失败，请重试');
    },

    // 加载数据
    async loadData(tabIndex, reset = true) {
      if (this.loadingLock[tabIndex]) return;
      
      this.$set(this.loadingLock, tabIndex, true);
      
      if (reset) {
        this.$set(this.pageMap, tabIndex, 1);
        this.$set(this.isLoading, tabIndex, true);
      } else {
        this.$set(this.loadStatus, tabIndex, 'loading');
      }

      try {
        const res = await uni.$u.http.request({
          url: '/api/order/orders',
          method: 'GET',
          params: {
            customerId: uni.getStorageSync('userInfo')?.id || 0,
            statusId: this.tabList[tabIndex].statusId,
            pageIndex: this.pageMap[tabIndex],
            pageSize: 10
          }
        });

        const list = res.data || [];
        
        if (reset) {
          this.$set(this.orderList, tabIndex, list);
        } else {
          this.$set(this.orderList, tabIndex, [...this.orderList[tabIndex], ...list]);
        }
        
        this.$set(this.loadStatus, tabIndex, list.length < 10 ? 'nomore' : 'loadmore');
        this.$set(this.pageMap, tabIndex, this.pageMap[tabIndex] + 1);
        
      } catch (error) {
        this.handleError(error, tabIndex);
      } finally {
        this.$set(this.loadingLock, tabIndex, false);
        this.$set(this.refresherTrigger, tabIndex, false);
        this.$set(this.isLoading, tabIndex, false);
      }
    },

    // 事件处理优化
    handleLoadMore() {
      this.loadData(this.current, false);
    },

    handleRefresh() {
      this.loadData(this.current, true);
    },

    handleTabChange(e) {
      const idx = typeof e === 'object' ? e.index : e;
      if (idx === this.current) return;
      
      this.current = idx;
      if (!this.orderList[idx] || this.orderList[idx].length === 0) {
        this.loadData(idx, true);
      }
    },

    handleSwiperChange(e) {
      const idx = e.detail.current;
      this.current = idx;
      if (!this.orderList[idx] || this.orderList[idx].length === 0) {
        this.loadData(idx, true);
      }
    },

    handleGoShopping() {
      uni.switchTab({ url: '/pages/index/index' });
    },

    handleOrderDetail(id) {
      uni.navigateTo({ url: `/pages/order/detail?id=${id}` });
    },

    // 状态样式
    getStatusClass(status) {
      //console.log('订单状态值:', status); // 调试输出
      const statusStr = String(status || '').trim();
      
      // 更灵活的状态映射
      if (statusStr.includes('待支付') || statusStr.includes('待付款') || statusStr === '1') {
        return 'status-pending';
      } else if (statusStr.includes('待发货') || statusStr === '2') {
        return 'status-processing';
      } else if (statusStr.includes('待收货') || statusStr === '3') {
        return 'status-shipped';
      } else if (statusStr.includes('已完成') || statusStr === '4') {
        return 'status-completed';
      } else if (statusStr.includes('已取消') || statusStr === '5') {
        return 'status-cancelled';
      } else if (statusStr.includes('退款中') || statusStr === '6') {
        return 'status-refunding';
      } else if (statusStr.includes('已退款') || statusStr === '7') {
        return 'status-refunded';
      }
      
      return '';
    },

    // 价格格式化
    formatPrice(price) {
      return Number(price).toFixed(2);
    },

    // 操作按钮配置优化 - 状态颜色区分
    getOrderActions(order) {
      const baseStyle = {
        width: '160rpx',
        height: '60rpx',
        lineHeight: '60rpx',
        fontSize: '26rpx'
      };

      const actions = [];
      const status = String(order.orderStatus || '').trim();
      
      // 调试输出实际状态值
      //console.log('订单状态处理:', status, '订单ID:', order.id);
      
      // 使用更灵活的匹配方式
      if (status.includes('待支付') || status.includes('待付款') || status === '1') {
        actions.push({
          text: '立即支付',
          type: 'primary',
          style: { ...baseStyle, backgroundColor: '#a855f7', color: '#fff' },
          click: () => this.handlePay(order.id)
        });
        actions.push({
          text: '取消订单',
          type: 'default',
          plain: true,
          style: baseStyle,
          click: () => this.handleCancelOrder(order)
        });
      } else if (status.includes('未发货') || status === '2') {
        actions.push({
          text: '催发货',
          type: 'warning',
          style: { ...baseStyle, backgroundColor: '#f59e0b', color: '#fff' },
          click: () => this.handleRemindShip(order.id)
        });
        actions.push({
          text: '申请退款',
          type: 'error',
          plain: true,
          style: { ...baseStyle, backgroundColor: '#ef4444', color: '#fff' },
          click: () => this.handleApplyRefund(order)
        });
      } else if (status.includes('已发货') || status === '3') {
        actions.push({
          text: '查看物流',
          type: 'info',
          style: { ...baseStyle, backgroundColor: '#06b6d4', color: '#fff' },
          click: () => this.handleLogistics(order.id)
        });
        actions.push({
          text: '确认收货',
          type: 'success',
          style: { ...baseStyle, backgroundColor: '#10b981', color: '#fff' },
          click: () => this.handleConfirmReceive(order)
        });
      } else if (status.includes('已完成') || status === '4') {
        actions.push({
          text: '评价',
          type: 'default',
          plain: true,
          style: baseStyle,
          click: () => this.handleEvaluate(order.id)
        });
        actions.push({
          text: '再次购买',
          type: 'primary',
          style: { ...baseStyle, backgroundColor: '#a855f7', color: '#fff' },
          click: () => this.handleBuyAgain(order)
        });
      } else if (status.includes('已取消') || status === '5') {
        actions.push({
          text: '删除订单',
          type: 'error',
          style: { ...baseStyle, backgroundColor: '#ef4444', color: '#fff' },
          click: () => this.handleDeleteOrder(order.id)
        });
      } else if (status.includes('退款中') || status === '6') {
        actions.push({
          text: '查看退款',
          type: 'info',
          style: { ...baseStyle, backgroundColor: '#8b5cf6', color: '#fff' },
          click: () => this.handleRefundDetail(order.id)
        });
      } else if (status.includes('已退款') || status === '7') {
        actions.push({
          text: '退款详情',
          type: 'info',
          style: { ...baseStyle, backgroundColor: '#06b6d4', color: '#fff' },
          click: () => this.handleRefundDetail(order.id)
        });
      } else {
        // 默认显示查看详情
        actions.push({
          text: '查看详情',
          type: 'default',
          plain: true,
          style: baseStyle,
          click: () => this.handleOrderDetail(order.id)
        });
      }
      
      return actions;
    },

    // 操作处理方法 - 修复所有API调用和事件处理
    async handlePay(id) {
      uni.navigateTo({ url: `/pages/pay/index?orderId=${id}` });
    },

    async handleRemindShip(id) {
      uni.showToast({
        title: '已提醒商家发货',
        icon: 'success'
      });
    },

    async handleCancelOrder(order) {
      try {
        await uni.request({
          url: `/api/order/cancel?id=${order.id}`,
          method: 'POST'
        });
        
        const list = [...this.orderList[this.current]];
        const index = list.findIndex(o => o.id === order.id);
        if (index > -1) {
          list[index].orderStatus = '已取消';
          this.$set(this.orderList, this.current, list);
        }
        
        uni.showToast({
          title: '订单已取消',
          icon: 'success'
        });
      } catch (error) {
        uni.showToast({
          title: '取消失败，请重试',
          icon: 'none'
        });
      }
    },

    async handleConfirmReceive(order) {
      try {
        await uni.request({
          url: `/api/order/confirm?id=${order.id}`,
          method: 'POST'
        });
        
        const list = [...this.orderList[this.current]];
        const index = list.findIndex(o => o.id === order.id);
        if (index > -1) {
          list[index].orderStatus = '已完成';
          this.$set(this.orderList, this.current, list);
        }
        
        uni.showToast({
          title: '收货成功',
          icon: 'success'
        });
      } catch (error) {
        uni.showToast({
          title: '操作失败，请重试',
          icon: 'none'
        });
      }
    },

    async handleDeleteOrder(id) {
      try {
        await uni.request({
          url: `/api/order/delete?id=${id}`,
          method: 'POST',
		   data: {}
        });
        
        const list = this.orderList[this.current].filter(o => o.id !== id);
        this.$set(this.orderList, this.current, list);
        
        uni.showToast({
          title: '删除成功',
          icon: 'success'
        });
      } catch (error) {
        uni.showToast({
          title: '删除失败，请重试',
          icon: 'none'
        });
      }
    },

    handleLogistics(id) {
      uni.showToast({
        title: '查看物流功能开发中',
        icon: 'none'
      });
    },

    handleEvaluate(id) {
		uni.navigateTo({ url:`/pages/order/review?id=${id}` });
    },

    handleBuyAgain(order) {
      const cart = order.items.map(i => ({
        skuId: i.skuId,
        quantity: i.quantity,
        price: i.price
      }));
      uni.setStorageSync('fastCart', cart);
      uni.navigateTo({ url: '/pages/order/create?mode=buyAgain' });
    },
    
    handleRefundDetail(id) {
      uni.navigateTo({ url: `/pages/order/refund-detail?id=${id}` });
    },

    handleApplyRefund(order) {
      uni.showToast({
        title: '退款申请功能开发中',
        icon: 'none'
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.page {
  background: #f8fafc;
  min-height: 100vh;
}

.tab-aurora {
  background: #f8fafc;
  padding: 20rpx 0;
  border-bottom: 1rpx solid #e2e8f0;
}

.swiper-box {
  width: 100%;
}

.scroll-area {
  height: 100%;
  padding: 20rpx 0;
}

.loading-skeleton {
  padding: 0 20rpx;
  
  .skeleton-card {
    margin-bottom: 20rpx;
    padding: 30rpx;
    background: #fff;
    border-radius: 24rpx;
    box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.04);
  }
}

.empty-box {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  
  .empty-img {
    width: 320rpx;
    height: 320rpx;
    opacity: 0.5;
  }
  
  .empty-txt {
    margin: 30rpx 0 40rpx;
    font-size: 28rpx;
    color: #64748b;
  }
  
  .empty-btn {
    padding: 16rpx 48rpx;
    background: #a855f7;
    color: #fff;
    border-radius: 50rpx;
    font-size: 28rpx;
    font-weight: 500;
    box-shadow: 0 4rpx 12rpx rgba(168, 85, 247, 0.3);
    
    &:active {
      transform: scale(0.98);
    }
  }
}

.order-card {
  margin: 0 20rpx 20rpx;
  background: #fff;
  border-radius: 24rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.04);
  transition: all 0.2s ease;
  
  &:active {
    transform: translateY(1rpx);
    box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.08);
  }
}

.card-header {
  display: flex;
  align-items: center;
  padding: 24rpx 32rpx;
  border-bottom: 1rpx solid #f1f5f9;
  
  .shop-name {
    margin-left: 16rpx;
    font-size: 30rpx;
    font-weight: 600;
    color: #1e293b;
  }
  
  .plaza-name {
    margin-left: 8rpx;
    font-size: 26rpx;
    color: #64748b;
  }
  
  .status-tag {
    margin-left: auto;
    font-size: 26rpx;
    font-weight: 500;
    padding: 4rpx 12rpx;
    border-radius: 12rpx;
    
    &.status-pending {
      color: #a855f7;
      background: rgba(168, 85, 247, 0.1);
    }
    
    &.status-processing {
      color: #f59e0b;
      background: rgba(245, 158, 11, 0.1);
    }
    
    &.status-shipped {
      color: #10b981;
      background: rgba(16, 185, 129, 0.1);
    }
    
    &.status-completed {
      color: #10b981;
      background: rgba(16, 185, 129, 0.1);
    }
    
    &.status-cancelled {
      color: #94a3b8;
      background: rgba(148, 163, 184, 0.1);
    }
    
    &.status-refunded {
      color: #06b6d4;
      background: rgba(6, 182, 212, 0.1);
    }
    
    &.status-refunding {
      color: #8b5cf6;
      background: rgba(139, 92, 246, 0.1);
    }
  }
}

.goods-box {
  display: flex;
  padding: 24rpx 32rpx;
  border-bottom: 1rpx solid #f1f5f9;
  
  .goods-info {
    flex: 1;
    margin-left: 20rpx;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    
    .title {
      font-size: 28rpx;
      color: #1e293b;
      line-height: 40rpx;
      font-weight: 400;
    }
    
    .spec {
      margin-top: 4rpx;
      font-size: 24rpx;
      color: #64748b;
    }
    
    .price-line {
      margin-top: 8rpx;
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      .price {
        font-size: 28rpx;
        color: #a855f7;
        font-weight: 600;
      }
      
      .num {
        font-size: 24rpx;
        color: #64748b;
      }
    }
  }
}

.total-line {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24rpx 32rpx;
  border-bottom: 1rpx solid #f1f5f9;
  font-size: 28rpx;
  color: #64748b;
  
  .total-txt {
    .total-price {
      color: #a855f7;
      font-size: 32rpx;
      font-weight: 600;
    }
  }
}

.action-box {
  display: flex;
  justify-content: flex-end;
  gap: 16rpx;
  padding: 24rpx 32rpx;
  
  .btn {
    padding: 12rpx 32rpx;
    font-size: 26rpx;
    border-radius: 50rpx;
    font-weight: 500;
    transition: all 0.2s ease;
    
    &:active {
      transform: scale(0.98);
    }
    
    &.primary {
      background: #a855f7;
      color: #fff;
      box-shadow: 0 2rpx 8rpx rgba(168, 85, 247, 0.3);
    }
    
    &.default {
      background: #f1f5f9;
      color: #475569;
    }
    
    &.error {
      background: #ef4444;
      color: #fff;
      box-shadow: 0 2rpx 8rpx rgba(239, 68, 68, 0.3);
    }
    
    &.warning {
      background: #f59e0b;
      color: #fff;
      box-shadow: 0 2rpx 8rpx rgba(245, 158, 11, 0.3);
    }
    
    &.success {
      background: #10b981;
      color: #fff;
      box-shadow: 0 2rpx 8rpx rgba(16, 185, 129, 0.3);
    }
  }
}
</style>