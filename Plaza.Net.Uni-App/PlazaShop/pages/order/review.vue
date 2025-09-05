<template>
  <view class="evaluate-page">
    <!-- 顶部导航 -->
    <u-navbar
      title="商品评价"
      :is-back="true"
      bg-color="#fff"
      title-color="#1e293b"
      back-icon-color="#1e293b"
      @left-click="uni.navigateBack()"
    />

    <!-- 订单信息 -->
    <view class="order-info-card">
      <view class="store-info">
        <u-icon name="bag" color="#a855f7" size="32" />
        <text class="store-name">{{ orderInfo.storeName }}</text>
      </view>
      <text class="order-number">订单编号：{{ orderInfo.orderNo }}</text>
    </view>

    <!-- 商品列表 -->
    <view class="goods-list">
      <view
        v-for="(goods, index) in goodsList"
        :key="index"
        class="goods-item"
      >
        <view class="goods-info">
          <u-image
            :src="goods.imageUrl"
            width="120rpx"
            height="120rpx"
            radius="12"
            mode="aspectFill"
          />
          <view class="goods-detail">
            <text class="goods-name u-line-2">{{ goods.title }}</text>
            <text class="goods-spec">{{ goods.spec }}</text>
            <text class="goods-price">¥{{ goods.price }}</text>
          </view>
        </view>

        <!-- 评分 -->
        <view class="goods-rate">
          <text class="rate-label">商品评分</text>
          <u-rate
            v-model="goods.rate"
            :count="5"
            active-color="#f59e0b"
            inactive-color="#e2e8f0"
            size="36"
            @change="rateChange($event, goods)"
          />
          <text class="rate-text">{{ goods.rateText }}</text>
        </view>

        <!-- 评价内容 -->
        <view class="goods-content">
          <u--textarea
            v-model="goods.content"
            placeholder="说说这款商品怎么样（选填）"
            maxlength="200"
            count
            border="none"
          />
        </view>

        <!-- 图片上传 —— 仿反馈页 -->
        <view class="form-item">
          <text class="label">上传图片（可选，最多9张）</text>
          <view class="upload-area">
            <!-- 本地/线上混合预览 -->
            <view
              v-for="(item, idx) in goods.previewList"
              :key="idx"
              class="img-preview"
            >
              <img
                v-if="item.isLocal"
                :src="item.src"
                class="preview-img"
              />
              <u-image
                v-else
                :src="item.src"
                width="160rpx"
                height="160rpx"
                radius="10rpx"
              />
              <u-icon
                name="close-circle"
                color="#ff3b30"
                size="36rpx"
                class="delete-img"
                @click="deleteGoodsImage(goods, idx)"
              />
            </view>

            <!-- 添加按钮 -->
            <view
              v-if="goods.previewList.length < 9"
              class="upload-btn"
              @click="chooseGoodsImage(goods)"
            >
              <u-icon name="camera" color="#999" size="48rpx" />
              <text class="upload-text">添加图片</text>
            </view>
          </view>
        </view>
      </view>
    </view>

    <!-- 底部提交 -->
    <view class="submit-section">
      <u-button
        type="primary"
        :loading="submitLoading"
        :disabled="!canSubmit"
        @click="submitEvaluate"
      >
        提交评价
      </u-button>
    </view>
  </view>
</template>

<script>
// 通用上传函数
const uploadFile = (filePath) => {
  return new Promise((resolve, reject) => {
    uni.uploadFile({
      url: 'http://localhost:5132/User/UploadImageReview?uploadType=review',
	  method:'POST',
      filePath,
      name: 'file',
      formData: {},
      success: (res) => {
        try {
          const data = JSON.parse(res.data);
          data.success ? resolve(data.imageUrl) : reject(data.message || '上传失败');
        } catch {
          reject('返回格式错误');
        }
      },
      fail: (err) => reject(err.errMsg || '网络错误')
    });
  });
};

export default {
  data() {
    return {
      orderId: '',
      orderInfo: { storeName: '', orderNo: '' },
      goodsList: [],
      submitLoading: false
    };
  },
  onLoad(options) {
    this.orderId = options.id || '';
    this.loadOrderDetail();
  },
  computed: {
    canSubmit() {
      return this.goodsList.length && this.goodsList.every(g => g.rate > 0);
    }
  },
  methods: {
    /* 获取订单详情 */
    loadOrderDetail() {
      if (!this.orderId) return uni.showToast({ title: '订单ID无效', icon: 'none' });
      uni.showLoading({ title: '加载中...' });
      uni.request({
       url: `/api/order/orderitem?id=${this.orderId}`,
        method: 'GET',
        success: (res) => {
          if (res.statusCode === 200 && res.data) {
            const dto = res.data;
            this.orderInfo = {
              storeName: dto.storeName || '未知店铺',
              orderNo: dto.orderNo || this.orderId
            };
            this.goodsList = dto.items.map(it => ({
              id: it.itemId,
              title: it.title,
              spec: it.spec,
              price: it.price,
              imageUrl: it.imageUrl || '/static/logo.png',
              rate: 0,
              rateText: '',
              content: '',
              previewList: [], // 本地+线上预览
              uploadImgs: []   // 仅线上地址
            }));
			console.log(res)
          } else {
            uni.showToast({ title: '订单数据为空', icon: 'none' });
          }
        },
        fail: () => uni.showToast({ title: '加载订单失败', icon: 'none' }),
        complete: () => uni.hideLoading()
      });
    },

    /* 评分变化 */
    rateChange(e, goods) {
      const texts = ['非常差', '差', '一般', '好', '非常好'];
      goods.rateText = texts[e - 1] || '';
    },

    /* 选图 -> 本地预览 -> 逐个上传 -> 替换线上地址 */
    async chooseGoodsImage(goods) {
      const remain = 9 - goods.previewList.length;
      if (remain <= 0) return;

      const res = await uni.chooseImage({ count: remain });
      const localPaths = res.tempFilePaths;

      // 1. 立即本地预览
      localPaths.forEach(src => {
        goods.previewList.push({ src, isLocal: true });
      });

      uni.showLoading({ title: '上传中...' });

      // 2. 逐个上传并替换
      try {
        for (let i = 0; i < localPaths.length; i++) {
          const serverUrl = await uploadFile(localPaths[i]);
          const idx = goods.previewList.findIndex(item => item.src === localPaths[i]);
          this.$set(goods.previewList, idx, { src: serverUrl, isLocal: false });
          goods.uploadImgs.push(serverUrl);
        }
      } catch (e) {
        uni.showToast({ title: String(e), icon: 'none' });
      } finally {
        uni.hideLoading();
      }
    },

    /* 删除图片 */
    deleteGoodsImage(goods, idx) {
      const url = goods.previewList[idx].src;
      goods.previewList.splice(idx, 1);
      const uploadIdx = goods.uploadImgs.indexOf(url);
      if (uploadIdx > -1) goods.uploadImgs.splice(uploadIdx, 1);
    },

    /* 提交评价 */
    async submitEvaluate() {
      if (!this.canSubmit) {
        uni.showToast({ title: '请完成所有评价', icon: 'none' });
        return;
      }
      this.submitLoading = true;

      const rateMap = { 1: 45, 2: 46, 3: 47, 4: 48, 5: 49 };
    const dtoList = this.goodsList.map(g => ({
      itemId: g.itemId,
      rate:   {1:45,2:46,3:47,4:48,5:49}[g.rate],
      content: g.content.trim(),
      imageUrls: g.uploadImgs.join(';'),
      userId: uni.getStorageSync('userInfo')?.id || 0
    }));
	  

      uni.request({
        url: '/api/order/reviews',
        method: 'POST',
        data: dtoList, 
        success: (res) => {
			console.log('状态码', res.statusCode);
			console.log('返回体', res.data);
          if (res.data?.code === 0) {
            uni.showToast({ title: '评价成功', icon: 'success' });
            setTimeout(() => uni.switchTab({
            	url: '/pages/order/list'
            }), 1500);
          } else {
            uni.showToast({ title: res.data?.msg || '提交失败', icon: 'none' });
          }
        },
        fail: () => uni.showToast({ title: '网络错误', icon: 'none' }),
        complete: () => this.submitLoading = false
      });
	 
    }
  }
};
</script>

<style lang="scss" scoped>
.evaluate-page {
  padding-bottom: 160rpx;
  background: #f8fafc;
}
.order-info-card {
  margin: 20rpx;
  padding: 32rpx;
  background: #fff;
  border-radius: 24rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.04);
  .store-info {
    display: flex;
    align-items: center;
    margin-bottom: 16rpx;
    .store-name {
      margin-left: 16rpx;
      font-size: 32rpx;
      font-weight: 600;
    }
  }
  .order-number {
    font-size: 28rpx;
    color: #64748b;
  }
}

.goods-list {
  margin: 0 20rpx;
  .goods-item {
    margin-bottom: 20rpx;
    background: #fff;
    border-radius: 24rpx;
    overflow: hidden;
    box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.04);
    .goods-info {
      display: flex;
      padding: 32rpx;
      border-bottom: 1rpx solid #f1f5f9;
      .goods-detail {
        flex: 1;
        margin-left: 24rpx;
        .goods-name {
          font-size: 28rpx;
          color: #1e293b;
          line-height: 40rpx;
        }
        .goods-spec {
          font-size: 24rpx;
          color: #64748b;
          margin: 8rpx 0;
        }
        .goods-price {
          font-size: 32rpx;
          color: #a855f7;
          font-weight: 600;
        }
      }
    }
    .goods-rate {
      display: flex;
      align-items: center;
      padding: 24rpx 32rpx;
      .rate-label {
        font-size: 28rpx;
        color: #1e293b;
        margin-right: 24rpx;
      }
      .rate-text {
        margin-left: 16rpx;
        font-size: 28rpx;
        color: #f59e0b;
        font-weight: 500;
      }
    }
    .goods-content {
      padding: 0 32rpx 24rpx;
    }

    /* 图片上传样式直接复用反馈页 */
    .form-item {
      padding: 0 32rpx 32rpx;
      .label {
        display: block;
        font-size: 28rpx;
        color: #333;
        margin-bottom: 15rpx;
      }
      .upload-area {
        display: flex;
        flex-wrap: wrap;
        gap: 20rpx;
      }
      .img-preview {
        position: relative;
        width: 160rpx;
        height: 160rpx;
      }
      .preview-img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        border-radius: 10rpx;
      }
      .delete-img {
        position: absolute;
        top: -10rpx;
        right: -10rpx;
        background: #fff;
        border-radius: 50%;
      }
      .upload-btn {
        width: 160rpx;
        height: 160rpx;
        border: 2rpx dashed #ccc;
        border-radius: 10rpx;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
      }
      .upload-text {
        font-size: 24rpx;
        color: #999;
        margin-top: 10rpx;
      }
    }
  }
}

.submit-section {
  position: fixed;
  left: 0;
  right: 0;
  bottom: 0;
  padding: 20rpx;
  background: #fff;
  box-shadow: 0 -2rpx 8rpx rgba(0, 0, 0, 0.04);
}
</style>