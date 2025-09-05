<template>
  <view class="feedback-submit-page">
    <!-- 顶部导航 -->
    <u-navbar
      title="提交反馈"
      left-icon="arrow-left"
      @left-click="handleBack"
      background="#3c9cff"
      title-color="#fff"
    />

    <view class="content-container">
      <!-- 反馈内容 -->
      <view class="form-item">
        <text class="label required">反馈内容</text>
        <u-input
          v-model="content"
          type="textarea"
          rows="5"
          maxlength="500"
          placeholder="请详细描述问题或建议..."
          border="none"
        />
        <text class="word-count">{{ content.length }}/500</text>
      </view>

      <!-- 图片上传 -->
      <view class="form-item">
        <text class="label">上传图片（可选，最多5张）</text>
        <view class="upload-area">
          <!-- 本地/线上混合预览 -->
          <view
            v-for="(item, index) in previewList"
            :key="index"
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
              @click="deleteImage(index)"
            />
          </view>

          <!-- 添加按钮 -->
          <view
            v-if="previewList.length < 5"
            class="upload-btn"
            @click="chooseImage"
          >
            <u-icon name="camera" color="#999" size="48rpx" />
            <text class="upload-text">添加图片</text>
          </view>
        </view>
      </view>

      <!-- 联系方式 -->
      <view class="form-item">
        <text class="label">联系方式（可选）</text>
        <u-input
          v-model="contact"
          placeholder="手机号或邮箱"
          border="none"
        />
      </view>

      <!-- 提交按钮 -->
      <u-button
        class="submit-btn"
        @click="submitFeedback"
        :disabled="!content.trim() || submitting"
        :loading="submitting"
        color="#3c9cff"
      >
        提交反馈
      </u-button>
    </view>
  </view>
</template>

<script>
// 上传封装：H5 必须把 uploadType 放查询串，formData 留空
const uploadFile = (filePath) => {
  return new Promise((resolve, reject) => {
    uni.uploadFile({
      url: 'http://localhost:5132/User/UploadImageFeedback?uploadType=feedback',
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
       content: '',            // 反馈内容
       contact: '',            // 联系方式
       userId: 0,              // 当前用户 id
       previewList: [],        // 本地预览 + 线上 url
       uploadImgs: [],         // 仅线上 url（提交用）
       submitting: false       // 提交 loading
     };
  },
   onLoad(options) {
      this.userId = Number(options.userId || 0);
       if (!this.userId) {
         // 兜底从缓存拿
         const user = uni.getStorageSync('userInfo') || {};
         this.userId = user.id || 0;
       }
	   console.log('当前 userId:', this.userId);
    },
  methods: {
    /* 返回上一页 */
    handleBack() {
      uni.navigateBack();
    },

    /* 选图 → 立即本地预览 → 上传 → 替换为线上地址 */
    async chooseImage() {
      const remain = 5 - this.previewList.length;
      if (remain <= 0) return;

      const res = await uni.chooseImage({ count: remain });
      const localPaths = res.tempFilePaths;

      // 1️⃣ 立即把本地 blob 塞进预览列表（本地不会消失）
      localPaths.forEach(src => {
        this.previewList.push({ src, isLocal: true });
      });

      uni.showLoading({ title: '上传中...' });

      // 2️⃣ 逐个上传并替换
      try {
        for (let i = 0; i < localPaths.length; i++) {
          const serverUrl = await uploadFile(localPaths[i]);
          const idx = this.previewList.findIndex(item => item.src === localPaths[i]);
          // 替换成本地 blob → 线上 url
          this.$set(this.previewList, idx, { src: serverUrl, isLocal: false });
          this.uploadImgs.push(serverUrl);   // 提交用
        }
      } catch (e) {
        uni.showToast({ title: String(e), icon: 'none' });
      } finally {
        uni.hideLoading();
      }
    },

    /* 删除图片（同时清理 uploadImgs） */
    deleteImage(index) {
      const url = this.previewList[index].src;
      this.previewList.splice(index, 1);

      const uploadIdx = this.uploadImgs.indexOf(url);
      if (uploadIdx > -1) this.uploadImgs.splice(uploadIdx, 1);
    },

    /* 提交反馈 */
    async submitFeedback() {
      if (!this.content.trim()) {
        uni.showToast({ title: '请输入反馈内容', icon: 'none' });
        return;
      }

      this.submitting = true;
      try {
        const res = await uni.request({
          method: 'POST',
          url: '/api/user/feedback',   // 代理到 5132 或改绝对地址
          data: {
            Content: this.content,
            Imageurls: this.uploadImgs.join(';'),
            Contact: this.contact,
            UserId: this.userId || 0
          }
        });
        if (res.data.success) {
          uni.showToast({ title: res.data.msg });
          setTimeout(() => uni.navigateBack(), 1500);
        } else {
          uni.showToast({ title: res.data.msg, icon: 'none' });
        }
      } catch {
        uni.showToast({ title: '网络错误', icon: 'none' });
      } finally {
        this.submitting = false;
      }
    }
  }
};
</script>

<style scoped lang="scss">
.feedback-submit-page {
  background-color: #f5f5f5;
  min-height: 100vh;
}
.content-container {
  padding: 30rpx 20rpx;
}
.form-item {
  background: #fff;
  border-radius: 15rpx;
  padding: 25rpx;
  margin-bottom: 20rpx;
}
.label {
  display: block;
  font-size: 28rpx;
  color: #333;
  margin-bottom: 15rpx;
}
.required::after {
  content: '*';
  color: #ff3b30;
  margin-left: 5rpx;
}
.word-count {
  display: block;
  text-align: right;
  font-size: 22rpx;
  color: #999;
  margin-top: 10rpx;
}
.upload-area {
  display: flex;
  flex-wrap: wrap;
  gap: 20rpx;
  margin-top: 15rpx;
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
.submit-btn {
  width: 100%;
  height: 90rpx;
  font-size: 30rpx;
  margin-top: 40rpx;
  border-radius: 45rpx;
}
</style>