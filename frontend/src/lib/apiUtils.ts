import { type PagedResult } from "../types/domain";

export type RawPagedResponse<T> = {
  items?: T[];
  Items?: T[];
  data?: T[];
  total?: number;
  Total?: number;
  totalCount?: number;
  TotalCount?: number;
  page?: number;
  Page?: number;
  pageIndex?: number;
  PageIndex?: number;
  pageSize?: number;
  PageSize?: number;
  page_size?: number;
};

export function normalizePagedResponse<T>(raw: unknown): PagedResult<T> {
  if (Array.isArray(raw)) {
    return { items: raw, total: raw.length, page: 1, pageSize: raw.length };
  }

  if (raw && typeof raw === "object") {
    const obj = raw as RawPagedResponse<T>;
    const maybeItems = obj.items ?? obj.Items ?? obj.data;
    const items = Array.isArray(maybeItems) ? maybeItems : [];
    const total =
      obj.total ?? obj.Total ?? obj.totalCount ?? obj.TotalCount ?? items.length;
    const page =
      obj.page ?? obj.Page ?? obj.pageIndex ?? obj.PageIndex ?? 1;
    const pageSize =
      obj.pageSize ?? obj.PageSize ?? obj.page_size ?? (items.length || 10);
    return { items, total, page, pageSize };
  }

  return { items: [], total: 0, page: 1, pageSize: 10 };
}